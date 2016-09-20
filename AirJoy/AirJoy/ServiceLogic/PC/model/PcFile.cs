using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AirJoy.ServiceLogic.PC.model
{
    /**
     * 文件的模板类
     * 
     * @author 赵庆洋
     * 
     */
    public class PcFile
    {
        
        //文件名； 
        public String fileName{set;get;}

        // 是否是目录；
        public bool directory{set;get;}

        // 是否是文件；
        public bool file{set;get;}

        // 文件路径（绝对路径）；
        public String filePath{set;get;}

        // 总空间
        public String totalSpace{set;get;}

        // 剩余空间
        public String freeSpace{set;get;}

        // 长度大小
        public long length{set;get;}

        // 上一级文件夹
        public String parent{set;get;}

        // 上一级文件夹绝对路径；
        public String parentPath{set;get;}

    }
}