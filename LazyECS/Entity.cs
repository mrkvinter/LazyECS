using System;
using System.Collections.Generic;
using System.Linq;
using LazyECS.Component;

namespace LazyECS
{
    public class Entity
    {
        public uint Id { get; }
        

        internal Entity(uint id)
        {
            Id = id;
        }

        public override bool Equals(object obj)
        {
            return obj is Entity other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int) Id;
        }

        public bool Equals(Entity entity)
        {
            return Id == entity.Id;
        }
    }
}