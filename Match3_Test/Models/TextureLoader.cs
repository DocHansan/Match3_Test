using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Match3_Test.Models
{
    static class TextureLoader
    {
        public static Texture LoadTexture(string path)
        {
            Texture Loaded_texture = new Texture(path);
            return Loaded_texture;
        }
    }
}
