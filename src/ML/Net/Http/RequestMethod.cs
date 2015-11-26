namespace ML.Net.Http
{
    public enum RequestMethod
    {
        OPTIONS = 0,

        HEAD = 1,

        GET = 2,

        POST = 3, // 這個請求可能会建立新的资源或修改現有资源，或二者皆有。

        PUT = 4, // 向指定资源位置上传其最新内容。

        DELETE = 5, // 请求服务器删除Request-URI所标识的资源。

        TRACE = 6, // 回显服务器收到的请求，主要用于测试或诊断。

        CONNECT = 7, // HTTP/1.1协议中预留给能够将连接改为管道方式的代理服务器。通常用於SSL加密伺服器的連結（經由非加密的HTTP代理伺服器）。

        PATCH = 8 //（由 RFC 5789 指定的方法）:用于将局部修改应用到资源。
    }
}
