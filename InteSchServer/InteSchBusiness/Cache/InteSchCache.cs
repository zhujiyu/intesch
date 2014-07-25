using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Memcached.ClientLibrary;

namespace InteSchBusiness.Cache
{
    public class InteSchCache
    {
        protected string[] servers = null;
        protected MemcachedClient mc = null;

        private void Init(string[] servers)
        {
            //初始化池
            SockIOPool sock = SockIOPool.GetInstance();
            sock.SetServers(servers);//添加服务器列表
            sock.InitConnections = 3;//设置连接池初始数目
            sock.MinConnections = 3;//设置最小连接数目
            sock.MaxConnections = 5;//设置最大连接数目
            sock.SocketConnectTimeout = 1000;//设置连接的套接字超时。
            sock.SocketTimeout = 3000;//设置套接字超时读取
            sock.MaintenanceSleep = 30;//设置维护线程运行的睡眠时间。如果设置为0，那么维护线程将不会启动;            
            //获取或设置池的故障标志。
            //如果这个标志被设置为true则socket连接失败，
            //将试图从另一台服务器返回一个套接字如果存在的话。
            //如果设置为false，则得到一个套接字如果存在的话。否则返回NULL，如果它无法连接到请求的服务器。
            sock.Failover = true; //如果为false，对所有创建的套接字关闭Nagle的算法。
            sock.Nagle = false;
            sock.Initialize();
        }

        public InteSchCache(string _server)
        {
            servers = new string[] { _server };
            try
            {
                Init(servers);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return;
            }
            mc = new MemcachedClient();//初始化一个客户端  
            mc.EnableCompression = true; //是否启用压缩数据
        }

        public InteSchCache(string[] _servers)
        {
            servers = _servers;
            try
            {
                Init(servers);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return;
            }
            mc = new MemcachedClient();//初始化一个客户端  
            mc.EnableCompression = true; //是否启用压缩数据
        }

        public void Flush()
        {
            mc.FlushAll();
        }

        protected object GetCache(string key)
        {
            return mc.Get(key);
        }

        protected void SetCache(string key, object value)
        {
            mc.Set(key, value);
        }
    }
}
