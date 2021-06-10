using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace onmyouchan.Entity
{
    class HitogataObject:Entity
    {
        public HitogataObject()
            : base(EntityType.attack)
        {
        }

        public override void Lord(EntityManager entityManager)
        {
            hitStatus = new List<EntityStatus_Hit>();
            hitStatus.Add(new EntityStatus_Hit_Item(this));
            base.Lord(entityManager);
        }
    }
    class EntityStatus_Hit_Item : EntityStatus_Hit_Ball
    {
        public EntityStatus_Hit_Item(Entity entity)
            : base(entity, 25, HitPriorityGroup.attack)
        { }
    }
}
