using System;
using System.Collections.Generic;
using System.Text;

namespace DeathNote
{
    internal class DeathNoteBehavior : PhysicsProp
    {
        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown)
            {

            }
        }
    }
}
