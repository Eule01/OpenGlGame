#region

using GameCore.Font;
using GameCore.UserInterface;
using OpenGL;

#endregion

namespace GameCore.RenderLayers
{
    public class RenderLayerTextInfo : RenderLayerBase
    {
        public bool ShowInfo = false;

        private BMFont font;
        private ShaderProgram fontProgram;
        private FontVAO information;
        public string GameInfo = "";


        public RenderLayerTextInfo(int width, int height, GameStatus theGameStatus, UserInputPlayer theUserInputPlayer,
                                   KeyBindings theKeyBindings)
            : base(width, height, theGameStatus, theUserInputPlayer, theKeyBindings)
        {
        }

        public override void OnLoad()
        {
            // load the bitmap font for this tutorial
            font = new BMFont(@".\Font\font24.fnt", @".\Font\font24.png");
//            font = new BMFont(@".\Render\OpenGl4CSharp\font24.fnt", @".\Render\OpenGl4CSharp\font24.png");
            fontProgram = new ShaderProgram(BMFont.FontVertexSource, BMFont.FontFragmentSource);

            fontProgram.Use();
            fontProgram["ortho_matrix"].SetValue(Matrix4.CreateOrthographic(Width, Height, 0, 10));
            fontProgram["color"].SetValue(new Vector3(1, 1, 1));

            information = font.CreateString(fontProgram, "Mikes Test Engine");
        }

        public override void OnDisplay()
        {
        }

        public override void OnRenderFrame(float deltaTime)
        {
            Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            // bind the font program as well as the font texture
            Gl.UseProgram(fontProgram.ProgramID);
            Gl.BindTexture(font.FontTexture);

            // draw the tutorial information, which is static
            information.Draw();


            // BUG The VBO not disposing error seems to come from in here.
            if (ShowInfo)
            {
                FontVAO gameOverlayInfo = font.CreateString(fontProgram, GameInfo,
                                                            BMFont.Justification.Right);

                gameOverlayInfo.Position = new Vector2(Width/2 - 10, Height/2 - font.Height - 10);
                gameOverlayInfo.Draw();
                gameOverlayInfo.Dispose();
            }
        }

        public override void OnReshape(int width, int height)
        {
            Width = width;
            Height = height;

            Gl.UseProgram(fontProgram.ProgramID);
            fontProgram["ortho_matrix"].SetValue(Matrix4.CreateOrthographic(width, height, 0, 1000));

            information.Position = new Vector2(-width/2 + 10, height/2 - font.Height - 10);
        }

        public override void OnClose()
        {
            fontProgram.DisposeChildren = true;
            fontProgram.Dispose();
            font.FontTexture.Dispose();
            information.Dispose();
        }

        public override bool OnMouse(int button, int state, int x, int y)
        {
            return false;
        }

        public override void OnMove(int x, int y)
        {
        }

        public override void OnSpecialKeyboardDown(int key, int x, int y)
        {
        }

        public override void OnSpecialKeyboardUp(int key, int x, int y)
        {
        }

        public override void OnKeyboardDown(byte key, int x, int y)
        {
        }

        public override void OnKeyboardUp(byte key, int x, int y)
        {
            if (key == TheKeyBindings.TheKeyLookUp[KeyBindings.Ids.DisplayToggleDisplayInfo]) ShowInfo = !ShowInfo;
        }
    }
}