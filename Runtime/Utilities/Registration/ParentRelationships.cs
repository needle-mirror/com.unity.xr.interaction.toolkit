using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.XR.CoreUtils.Collections;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities.Registration
{
    class ParentRelationships<TEntity, TParent>
    {
        class EntityRelationship
        {
            public TEntity entity { get; }

            [CanBeNull]
            public HashSetList<TParent> explicitParents { get; private set; }

            [CanBeNull]
            public HashSetList<TParent> inheritedParents { get; private set; }

            public EntityRelationship(TEntity entity)
            {
                this.entity = entity;
            }

            public bool AddExplicitParent(TParent parent)
            {
                explicitParents ??= new HashSetList<TParent>();
                return explicitParents.Add(parent);
            }

            public bool AddInheritedParent(TParent parent)
            {
                inheritedParents ??= new HashSetList<TParent>();
                return inheritedParents.Add(parent);
            }

            public bool RemoveExplicitParent(TParent parent)
            {
                return explicitParents != null && explicitParents.Remove(parent);
            }

            public bool RemoveInheritedParent(TParent parent)
            {
                return inheritedParents != null && inheritedParents.Remove(parent);
            }

            public bool HasParent(TParent parent)
            {
                return (explicitParents != null && explicitParents.Contains(parent)) ||
                    (inheritedParents != null && inheritedParents.Contains(parent));
            }
        }

        class ReverseRelationship
        {
            public TParent entity { get; }

            [CanBeNull]
            public HashSetList<TEntity> explicitChildren { get; private set; }

            [CanBeNull]
            public HashSetList<TEntity> inheritedChildren { get; private set; }

            public bool HasAnyChild => (explicitChildren != null && explicitChildren.Count > 0) || (inheritedChildren != null && inheritedChildren.Count > 0);

            public ReverseRelationship(TParent entity)
            {
                this.entity = entity;
            }

            public bool AddExplicitChild(TEntity child)
            {
                explicitChildren ??= new HashSetList<TEntity>();
                return explicitChildren.Add(child);
            }

            public bool AddInheritedChild(TEntity child)
            {
                inheritedChildren ??= new HashSetList<TEntity>();
                return inheritedChildren.Add(child);
            }

            public bool RemoveExplicitChild(TEntity child)
            {
                return explicitChildren != null && explicitChildren.Remove(child);
            }

            public bool RemoveInheritedChild(TEntity child)
            {
                return inheritedChildren != null && inheritedChildren.Remove(child);
            }
        }

        readonly Dictionary<TEntity, EntityRelationship> m_Entities = new Dictionary<TEntity, EntityRelationship>();
        readonly Dictionary<TParent, ReverseRelationship> m_ReverseEntities = new Dictionary<TParent, ReverseRelationship>();

        static List<TEntity> s_EntitiesToPrune;
        static List<TParent> s_ReverseEntitiesToPrune;

        public bool AddExplicitParent(TEntity entity, TParent parent)
        {
            var entityRelationship = GetOrAddRelationship(entity);
            if (entityRelationship.AddExplicitParent(parent))
            {
                var reverseRelationship = GetOrAddReverseRelationship(parent);
                reverseRelationship.AddExplicitChild(entity);

                return true;
            }

            return false;
        }

        public bool AddInheritedParent(TEntity entity, TParent parent)
        {
            var entityRelationship = GetOrAddRelationship(entity);
            if (entityRelationship.AddInheritedParent(parent))
            {
                var reverseRelationship = GetOrAddReverseRelationship(parent);
                reverseRelationship.AddInheritedChild(entity);

                return true;
            }

            return false;
        }

        public bool RemoveExplicitParent(TEntity entity, TParent parent)
        {
            if (m_Entities.TryGetValue(entity, out var entityRelationship) && entityRelationship.RemoveExplicitParent(parent))
            {
                if (m_ReverseEntities.TryGetValue(parent, out var reverseRelationship))
                    reverseRelationship.RemoveExplicitChild(entity);

                return true;
            }

            return false;
        }

        public bool RemoveInheritedParent(TEntity entity, TParent parent)
        {
            if (m_Entities.TryGetValue(entity, out var entityRelationship) && entityRelationship.RemoveInheritedParent(parent))
            {
                if (m_ReverseEntities.TryGetValue(parent, out var reverseRelationship))
                    reverseRelationship.RemoveInheritedChild(entity);

                return true;
            }

            return false;
        }

        public void PruneEntity(TEntity entity)
        {
            if (m_Entities.TryGetValue(entity, out var entityRelationship))
            {
                PruneEntity(entityRelationship);
                m_Entities.Remove(entity);
            }
        }

        public void PruneParent(TParent entity)
        {
            if (m_ReverseEntities.TryGetValue(entity, out var reverseRelationship))
            {
                PruneParent(reverseRelationship);
                m_ReverseEntities.Remove(entity);
            }
        }

        public bool HasParent(TEntity entity, TParent parent)
        {
            return m_Entities.TryGetValue(entity, out var entityRelationship) && entityRelationship.HasParent(parent);
        }

        public bool IsParent(TParent entity)
        {
            return m_ReverseEntities.TryGetValue(entity, out var reverseRelationship) && reverseRelationship.HasAnyChild;
        }

        public bool TryGetParents(TEntity entity, out HashSetList<TParent> explicitParents)
        {
            if (m_Entities.TryGetValue(entity, out var entityRelationship))
            {
                explicitParents = entityRelationship.explicitParents;
                return true;
            }

            explicitParents = null;
            return false;
        }

        public bool TryGetParents(TEntity entity, out HashSetList<TParent> explicitParents, out HashSetList<TParent> inheritedParents)
        {
            if (m_Entities.TryGetValue(entity, out var entityRelationship))
            {
                explicitParents = entityRelationship.explicitParents;
                inheritedParents = entityRelationship.inheritedParents;
                return true;
            }

            explicitParents = null;
            inheritedParents = null;
            return false;
        }

        public void GetParents(TEntity entity, List<TParent> explicitParents)
        {
            explicitParents.Clear();

            if (m_Entities.TryGetValue(entity, out var entityRelationship))
            {
                if (entityRelationship.explicitParents != null)
                    explicitParents.AddRange(entityRelationship.explicitParents);

            }
        }

        public void GetParents(TEntity entity, List<TParent> explicitParents, List<TParent> inheritedParents)
        {
            explicitParents.Clear();
            inheritedParents.Clear();

            if (m_Entities.TryGetValue(entity, out var entityRelationship))
            {
                if (entityRelationship.explicitParents != null)
                    explicitParents.AddRange(entityRelationship.explicitParents);

                if (entityRelationship.inheritedParents != null)
                    inheritedParents.AddRange(entityRelationship.inheritedParents);
            }
        }

        void PruneEntity(EntityRelationship entityRelationship)
        {
            if (entityRelationship.explicitParents != null && entityRelationship.explicitParents.Count > 0)
            {
                foreach (var parent in entityRelationship.explicitParents)
                {
                    if (m_ReverseEntities.TryGetValue(parent, out var reverseRelationship))
                        reverseRelationship.RemoveExplicitChild(entityRelationship.entity);
                }
            }

            if (entityRelationship.inheritedParents != null && entityRelationship.inheritedParents.Count > 0)
            {
                foreach (var parent in entityRelationship.inheritedParents)
                {
                    if (m_ReverseEntities.TryGetValue(parent, out var reverseRelationship))
                        reverseRelationship.RemoveInheritedChild(entityRelationship.entity);
                }
            }
        }

        void PruneParent(ReverseRelationship reverseRelationship)
        {
            if (reverseRelationship.explicitChildren != null && reverseRelationship.explicitChildren.Count > 0)
            {
                foreach (var child in reverseRelationship.explicitChildren)
                {
                    if (m_Entities.TryGetValue(child, out var entityRelationship))
                        entityRelationship.RemoveExplicitParent(reverseRelationship.entity);
                }
            }

            if (reverseRelationship.inheritedChildren != null && reverseRelationship.inheritedChildren.Count > 0)
            {
                foreach (var child in reverseRelationship.inheritedChildren)
                {
                    if (m_Entities.TryGetValue(child, out var entityRelationship))
                        entityRelationship.RemoveInheritedParent(reverseRelationship.entity);
                }
            }
        }

        EntityRelationship GetOrAddRelationship(TEntity entity)
        {
            if (!m_Entities.TryGetValue(entity, out var entityRelationship))
            {
                entityRelationship = new EntityRelationship(entity);
                m_Entities.Add(entity, entityRelationship);
            }

            return entityRelationship;
        }

        ReverseRelationship GetOrAddReverseRelationship(TParent entity)
        {
            if (!m_ReverseEntities.TryGetValue(entity, out var reverseRelationship))
            {
                reverseRelationship = new ReverseRelationship(entity);
                m_ReverseEntities.Add(entity, reverseRelationship);
            }

            return reverseRelationship;
        }
    }
}
