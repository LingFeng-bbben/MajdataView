using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class StarLogger : MonoBehaviour
{
    bool logEnabled = false;

    string slide_content;
    FileStream stream;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setSlideContent(string slideContent)
    {
        if (logEnabled)
        {
            slideContent = slideContent.Replace("<", "l").Replace(">", "r");
            slideContent = slideContent.Substring(0, slideContent.Length - 5);
            stream = new FileStream("D:\\code\\python\\2022\\maidataŒﬁ¿ÌºÏ≤‚\\pass\\"+slideContent+".txt", FileMode.Create);
        }
    }

    public void saveLog()
    {
        if (logEnabled)
        {
            stream.Close();
        }
    }

    public void Log(string content)
    {
        if (logEnabled)
        { 
            byte[] bytes = new UTF8Encoding(true).GetBytes(content+"\n");
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
