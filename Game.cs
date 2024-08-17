using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace prueba1
{
    public class Game : GameWindow
    {
        private int vertexbufferObject;
        private int elementBufferObject;
        private int shaderProgramObject;
        private int vertexArrayObject;
        private Matrix4 model;
        private Matrix4 view;
        private Matrix4 projection;
        private float rotationAngle;
        private float rotationSpeed = 0.0f; 

        public Game()
            : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            CenterWindow(new Vector2i(800, 640));
        }

        protected override void OnLoad()
        {
            GL.ClearColor(new Color4(0.0f, 0.5f, 0.0f, 1f));

            float[] vertices = new float[]
            { 
                // Posiciones (x, y, z) y Colores (r, g, b)
                // Primer cubo (parte horizontal de la T)
                -0.50f,  0.50f,  0.15f,  0.0f, 0.0f, 0.0f,  // Vértice 0: Superior izquierdo frontal - Rojo
                 0.50f,  0.50f,  0.15f,  0.0f, 0.0f, 0.0f,  // Vértice 1: Superior derecho frontal - Rojo
                 0.50f,  0.20f,  0.15f,  0.0f, 0.0f, 0.0f,  // Vértice 2: Inferior derecho frontal - Rojo
                -0.50f,  0.20f,  0.15f,  0.0f, 0.0f, 0.0f,  // Vértice 3: Inferior izquierdo frontal - Rojo

                -0.50f,  0.50f, -1.0f,  1.0f, 0.0f, 0.0f,  // Vértice 4: Superior izquierdo trasero - Rojo
                 0.50f,  0.50f, -1.0f,  1.0f, 0.0f, 0.0f,  // Vértice 5: Superior derecho trasero - Rojo
                 0.50f,  0.20f, -1.0f,  1.0f, 0.0f, 0.0f,  // Vértice 6: Inferior derecho trasero - Rojo
                -0.50f,  0.20f, -1.0f,  1.0f, 0.0f, 0.0f,  // Vértice 7: Inferior izquierdo trasero - Rojo
                
                // Segundo cubo (parte vertical de la T)
                -0.20f,  0.20f,  0.15f,  0.0f, 0.0f, 0.0f,  // Vértice 8: Superior izquierdo frontal - Verde
                 0.20f,  0.20f,  0.15f,  0.0f, 0.0f, 0.0f,  // Vértice 9: Superior derecho frontal - Verde
                 0.20f, -0.60f,  0.15f,  0.0f, 0.0f, 0.0f,  // Vértice 10: Inferior derecho frontal - Verde
                -0.20f, -0.60f,  0.15f,  0.0f, 0.0f, 0.0f,  // Vértice 11: Inferior izquierdo frontal - Verde

                -0.20f,  0.20f, -1.0f,  1.0f, 0.0f, 0.0f,  // Vértice 12: Superior izquierdo trasero - Verde
                 0.20f,  0.20f, -1.0f,  1.0f, 0.0f, 0.0f,  // Vértice 13: Superior derecho trasero - Verde
                 0.20f, -0.60f, -1.0f,  1.0f, 0.0f, 0.0f,  // Vértice 14: Inferior derecho trasero - Verde
                -0.20f, -0.60f, -1.0f,  1.0f, 0.0f, 0.0f   // Vértice 15: Inferior izquierdo trasero - Verde
                
            };

            uint[] indices = new uint[]
            {
                // Primer cubo
                0, 1, 2, 0, 2, 3,    // Cara frontal
                4, 5, 6, 4, 6, 7,    // Cara trasera
                0, 4, 7, 0, 7, 3,    // Cara izquierda
                1, 5, 6, 1, 6, 2,    // Cara derecha
                0, 1, 5, 0, 5, 4,    // Cara superior
                3, 2, 6, 3, 6, 7,    // Cara inferior

                // Segundo cubo
                8, 9, 10, 8, 10, 11, // Cara frontal
                12, 13, 14, 12, 14, 15, // Cara trasera
                8, 12, 15, 8, 15, 11,   // Cara izquierda
                9, 13, 14, 9, 14, 10,   // Cara derecha
                8, 9, 13, 8, 13, 12,    // Cara superior
                11, 10, 14, 11, 14, 15  // Cara inferior
            };

            vertexbufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexbufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexbufferObject);

            // Configura las posiciones
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Configura los colores
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
            GL.BindVertexArray(0);

            string vertexShaderCode =
                @"
                 #version 330 core
                 layout (location=0) in vec3 aPosition;               
                 layout (location=1) in vec3 aColor;
                 out vec3 vertexColor;
                 uniform mat4 model;
                 uniform mat4 view;
                 uniform mat4 projection;
                 void main(){
                 gl_Position = projection * view * model * vec4(aPosition, 1.0);
                 vertexColor = aColor;
                 }";

            string pixelShaderCode =
                @"
                 #version 330 core
                 in vec3 vertexColor;
                 out vec4 pixelColor;
                 void main(){
                 pixelColor=vec4(vertexColor, 1.0);
                 }
                 ";

            int vertexShaderObject = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderObject, vertexShaderCode);
            GL.CompileShader(vertexShaderObject);

            int pixelShaderObject = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(pixelShaderObject, pixelShaderCode);
            GL.CompileShader(pixelShaderObject);

            shaderProgramObject = GL.CreateProgram();
            GL.AttachShader(shaderProgramObject, vertexShaderObject);
            GL.AttachShader(shaderProgramObject, pixelShaderObject);
            GL.LinkProgram(shaderProgramObject);
            GL.DetachShader(shaderProgramObject, vertexShaderObject);
            GL.DetachShader(shaderProgramObject, pixelShaderObject);
            GL.DeleteShader(vertexShaderObject);
            GL.DeleteShader(pixelShaderObject);

            // Configurar las matrices de transformación
            model = Matrix4.Identity;
            view = Matrix4.LookAt(new Vector3(0, 0, 3), Vector3.Zero, Vector3.UnitY);
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), Size.X / (float)Size.Y, 0.1f, 100.0f);

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            base.OnLoad();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            rotationAngle += rotationSpeed * (float)args.Time;
            model = Matrix4.CreateRotationY(rotationAngle) * Matrix4.CreateRotationX(rotationAngle / 2);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(shaderProgramObject);
            GL.BindVertexArray(vertexArrayObject);

            int modelLocation = GL.GetUniformLocation(shaderProgramObject, "model");
            int viewLocation = GL.GetUniformLocation(shaderProgramObject, "view");
            int projectionLocation = GL.GetUniformLocation(shaderProgramObject, "projection");

            GL.UniformMatrix4(modelLocation, false, ref model);
            GL.UniformMatrix4(viewLocation, false, ref view);
            GL.UniformMatrix4(projectionLocation, false, ref projection);

            // Dibujar los cubos
            GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 0);
            GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, IntPtr.Add(IntPtr.Zero, 36 * sizeof(uint)));

            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Up))
            {
                rotationSpeed += 0.1f;
            }

            if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Down))
            {
                rotationSpeed = Math.Max(0.1f, rotationSpeed - 0.1f);
            }

            base.OnUpdateFrame(args);
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(vertexbufferObject);
            GL.DeleteBuffer(elementBufferObject);
            GL.UseProgram(0);
            GL.DeleteProgram(shaderProgramObject);
            base.OnUnload();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), e.Width / (float)e.Height, 0.1f, 100.0f);
            base.OnResize(e);
        }
    }
}