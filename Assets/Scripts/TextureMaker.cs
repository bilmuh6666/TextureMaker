using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Applications.Slots.Common;
using UnityEngine;

namespace Applications.Slots
{
    public class TextureMaker : MonoBehaviour
    {
        public SpriteRenderer[] rend;
        private Texture2D myTexture;
        private Texture2D alphaTexture;
        public Texture2D[] myTextures;
        public int counter;

        public Material mat;

        public List<Texture2D> textures;


        private void Awake()
        {
            EventServices.AddressableLoad.IconTextureLoaded += IconLoaded;
        }

        private void OnDestroy()
        {
            EventServices.AddressableLoad.IconTextureLoaded -= IconLoaded;
        }

        public void IconLoaded()
        {
            textures = new List<Texture2D>();
            textures = ModeLoader.Instance.iconTextures;
            CreateData();
        }


        private void RandomArray(List<Texture2D> array)
        {
            for (int i = 0; i < array.Count; i++)
            {
                Texture2D t = array[i];
                int r = RandomNumber(array.Count - i);
                array[i] = array[i + r];
                array[i + r] = t;
            }
        }

        private int RandomNumber(int max)
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] data = new byte[4];
                rng.GetBytes(data);
                int value = Math.Abs(BitConverter.ToInt32(data, 0)) % max;
                return value;
            }
        }

        private void CreateData()
        {
            RandomArray(textures);
            CalculateNewTextureSize();
        }

        public void TextureLayer()
        {
            Color32[] colorArray = new Color32[myTexture.height * myTexture.width];
            Color32[][] scrArray = new Color32[myTextures.Length][];

            for (int i = 0; i < myTextures.Length; i++)
            {
                scrArray[i] = myTextures[i].GetPixels32();
            }

            int a = 0;
            for (int i = 0; i < scrArray.Length; i++)
            {
                for (int j = 0; j < (scrArray[i]).Length; j++)
                {
                    Color32 scrPixel = scrArray[i][j];
                    colorArray[a] = scrPixel;
                    a++;
                }
            }

            myTexture.SetPixels32(colorArray);
            // myTexture.alphaIsTransparency = true;

            myTexture.Apply();
            myTexture.wrapMode = TextureWrapMode.Repeat;
            myTexture.filterMode = FilterMode.Bilinear;
            myTexture.name = "dene";
            myTexture.anisoLevel = 1;

            Sprite newSprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height),
                new Vector2(0.5f, 0.5f), 100f, 1, SpriteMeshType.FullRect);
            rend[counter].sprite = newSprite;
            rend[counter].sprite.name = "deneme";
            rend[counter].material = mat;

            if (counter < rend.Length - 1)
            {
                counter++;
                CreateData();
            }
        }

        public List<Sprite> sampleAvatars;
        public List<Sprite> sampleAvatarsDownsized;

        internal void InitTextures()
        {
            sampleAvatarsDownsized = new List<Sprite>(sampleAvatars.Count);
            for (int i = 0; i < sampleAvatars.Count; i++)
            {
                var avatar = sampleAvatars[i];
                var mipPixels = avatar.texture.GetPixels32(Mathf.Min(2, avatar.texture.mipmapCount));
                int len = (int)Math.Sqrt(mipPixels.Length);
                var t = new Texture2D(len, len, TextureFormat.RGBA32, false);
                t.SetPixels32(mipPixels);
                t.Apply();
                var sprite = Sprite.Create(t, new Rect(0, 0, len, len), Vector2.one * .5f);
                sampleAvatarsDownsized.Add(sprite);

                var noMipMapTex = new Texture2D(avatar.texture.width, avatar.texture.height, TextureFormat.RGBA32,
                    false);
                noMipMapTex.SetPixels32(avatar.texture.GetPixels32());
                noMipMapTex.Apply();
                sampleAvatars[i] = Sprite.Create(noMipMapTex, avatar.textureRect, Vector2.one * .5f);
            }
        }

        public void CalculateNewTextureSize()
        {
            int maxWidth = MaxWidth(textures);

            TextureAlpha(maxWidth);

            int totalHeight = TotalHeight(myTextures);
            myTexture = new Texture2D(maxWidth, totalHeight, TextureFormat.RGBA32, false);

            TextureLayer();
        }

        private int MaxWidth(List<Texture2D> numbers)
        {
            int max = numbers[0].width;

            for (int i = 0; i < numbers.Count; i++)
            {
                if (numbers[i] == null)
                    continue;

                if (max < numbers[i].width)
                    max = numbers[i].width;
            }

            return max;
        }

        public int TotalHeight(Texture2D[] numbers)
        {
            int totalHeight = 0;

            for (int i = 0; i < numbers.Length; i++)
            {
                totalHeight += numbers[i].height;
            }

            return totalHeight;
        }

        public void TextureAlpha(int myTextureWidth)
        {
            alphaTexture = new Texture2D(myTextureWidth, 100);

            Color[] textureColors = new Color[alphaTexture.width * alphaTexture.height];

            for (int y = 0; y < alphaTexture.height; y++)
            {
                for (int x = 0; x < alphaTexture.width; x++)
                {
                    textureColors[y + (x * alphaTexture.height)] = new Color(0, 0, 0, 0);
                }
            }

            alphaTexture.SetPixels(textureColors);
            alphaTexture.Apply();
            alphaTexture.wrapMode = TextureWrapMode.Clamp;
            alphaTexture.filterMode = FilterMode.Point;
            alphaTexture.name = "alphaTexture";

            CreateTextureArray();
        }

        public void CreateTextureArray()
        {
            int textureIndexSize = textures.Count + (textures.Count);
            myTextures = new Texture2D[textureIndexSize];

            int counter = 0;
            for (int i = 0; i < myTextures.Length; i++)
            {
                if (i % 2 == 1)
                {
                    myTextures[i] = textures[counter];
                    counter++;
                }
                else
                {
                    myTextures[i] = alphaTexture;
                }
            }
        }
    }
}