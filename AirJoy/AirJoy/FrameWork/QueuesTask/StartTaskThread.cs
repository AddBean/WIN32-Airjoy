using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AirJoy.FrameWork.QueuesTask
{
    class StartTaskThread
    {
        //开启多个线程执行队列中的任务，那就是先到先得，先处理；
        public static void RunAllThread(int ThreadNumber)
        {
            TaskThread thread = new TaskThread();
            for (int i = 0; i < ThreadNumber; i++)
            {
                Thread t=new Thread(thread.run); //开始执行时，队列为空，处于等待状态
                t.Name = "线程" + i;
                t.IsBackground = true;
                t.Start();
            }
        }
    }
}
