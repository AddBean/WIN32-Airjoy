using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AirJoy.FrameWork.QueuesTask
{
    public class TaskThread
    {
        public void run()
        {
            while (true)
            {
                lock (Queues.queue)
                {
                    while (Queues.queue.Count==0)
                    { //
                        try
                        {
                            //Queues.queue.; // 队列为空时，使线程处于等待状态
                          // Queues.queue.Thread.Sleep(1);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                    Queues.Task t = Queues.queue[0];// 得到第一个
                         Queues.queue.RemoveAt(0);//删除第一个；
                    try{
                         t.RunTask(); // 执行该任务
                    }
                    catch(Exception error)
                    {
                        Log.WriteLogInf("EVENT-ERROR-Thread", Thread.CurrentThread.Name+"线程创建失败：" + error.Message);//log记录；

                    }
                   
                }
            }
        }

    }
}
