using System.Collections.Generic;

namespace ReconoceImagenWebApp
{
    public class ImageAnalisysResult
    {
        public CaptionResult captionResult { get; set; }
        public List<Tag> tags { get; set; }
        public List<Object> objects { get; set; }
        // Agrega más propiedades según necesites (por ejemplo, readResult, people, etc.)
    }

    public class CaptionResult
    {
        public string text { get; set; }
        public float confidence { get; set; }
    }

    public class Tag
    {
        public string name { get; set; }
        public float confidence { get; set; }
    }

    public class Object
    {
        public string objectName { get; set; }
        public Rectangle rectangle { get; set; }
        public float confidence { get; set; }
    }

    public class Rectangle
    {
        public int x { get; set; }
        public int y { get; set; }
        public int w { get; set; }
        public int h { get; set; }
    }
}