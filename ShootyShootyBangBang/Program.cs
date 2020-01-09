using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBang
{
    class Program
    {
        static void Main(string[] args)
        {
            var controllers = new ShootyShootyBangBangEngine.Controllers.RenderControllers();
            controllers.SetCamera(new ShootyShootyBangBangEngine.GameObjects.Cameras.Camera(new OpenTK.Vector2()));
            using (ShootyShootyBangBangEngine.SSBBE engine = new ShootyShootyBangBangEngine.SSBBE(new ShootyShootyBangBangGame(controllers), 800, 600, "ShootyShootyBangBang"))
            {
                engine.Run(60.0);
            }
        }
    }
}
