using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AirJoy.ServiceLogic.PC.model
{
    /**
     * 文件上传记录
     * 
     * @author 赵庆洋
     * 
     */
    public class FileLog
    {

        private long id;

        private String path;

        private String tempPath;

        public long getId()
        {
            return id;
        }

        public void setId(long id)
        {
            this.id = id;
        }

        public String getPath()
        {
            return path;
        }

        public void setPath(String path)
        {
            this.path = path;
        }

        public String getTempPath()
        {
            return tempPath;
        }

        public void setTempPath(String tempPath)
        {
            this.tempPath = tempPath;
        }
    }
}