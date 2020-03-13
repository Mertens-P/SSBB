using OpenTK;
using OpenTK.Input;
using ShootyShootyBangBangEngine;
using ShootyShootyBangBangEngine.GameObjects.Cameras;
using ShootyShootyBangBangEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBang
{
    class ShootyShootyBangBangGame : ShootyShootyBangBangEngine.Game
    {
        TexturedQuad m_background;
        public ShootyShootyBangBangGame(ShootyShootyBangBangEngine.Controllers.BaseControllers controllers)
            :base(controllers)
        {

        }
    }
}
