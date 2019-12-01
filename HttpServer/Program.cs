using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace HttpServer {
    class Program {
        static void Main(string[] args) {

            // このデータはMDNを参照したものです
            // 何故かWindowsから取得はjsが無かったから適当に参考にして作りました
            var extentions = new Dictionary<string, string>() {
                {".aac","audio/aac"},
                {".abw","application/x-abiword"},
                {".arc","application/x-freearc"},
                {".avi","video/x-msvideo"},
                {".azw","application/vnd.amazon.ebook"},
                {".bin","application/octet-stream"},
                {".bmp","image/bmp"},
                {".bz","application/x-bzip"},
                {".bz2","application/x-bzip2"},
                {".csh","application/x-csh"},
                {".css","text/css"},
                {".csv","text/csv"},
                {".doc","application/msword"},
                {".docx","application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {".eot","application/vnd.ms-fontobject"},
                {".epub","application/epub+zip"},
                {".gz","application/gzip"},
                {".gif","image/gif"},
                {".htm","text/html"},
                {".html","text/html"},
                {".ico","image/vnd.microsoft.icon"},
                {".ics","text/calendar"},
                {".jar","application/java-archive"},
                {".jpeg","image/jpeg"},
                {".jpg","image/jpeg"},
                {".js","text/javascript"},
                {".json","application/json"},
                {".jsonld","application/ld+json"},
                {".mid","audio/midi audio/x-midi"},
                {".midi","audio/midi audio/x-midi"},
                {".mjs","text/javascript"},
                {".mp3","audio/mpeg"},
                {".mpeg","video/mpeg"},
                {".mpkg","application/vnd.apple.installer+xml"},
                {".odp","application/vnd.oasis.opendocument.presentation"},
                {".ods","application/vnd.oasis.opendocument.spreadsheet"},
                {".odt","application/vnd.oasis.opendocument.text"},
                {".oga","audio/ogg"},
                {".ogv","video/ogg"},
                {".ogx","application/ogg"},
                {".otf","font/otf"},
                {".png","image/png"},
                {".pdf","application/pdf"},
                {".php","appliction/php"},
                {".ppt","application/vnd.ms-powerpoint"},
                {".pptx","application/vnd.openxmlformats-officedocument.presentationml.presentation"},
                {".rar","application/x-rar-compressed"},
                {".rtf","application/rtf"},
                {".sh","application/x-sh"},
                {".svg","image/svg+xml"},
                {".swf","application/x-shockwave-flash"},
                {".tar","application/x-tar"},
                {".tif","image/tiff"},
                {".tiff","image/tiff"},
                {".ts","video/mp2t"},
                {".ttf","font/ttf"},
                {".txt","text/plain"},
                {".vsd","application/vnd.visio"},
                {".wav","audio/wav"},
                {".weba","audio/webm"},
                {".webm","video/webm"},
                {".webp","image/webp"},
                {".woff","font/woff"},
                {".woff2","font/woff2"},
                {".xhtml","application/xhtml+xml"},
                {".xls","application/vnd.ms-excel"},
                {".xlsx","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".xml","application/xml"},
                {".xul","application/vnd.mozilla.xul+xml"},
                {".zip","application/zip"},
                {".3gp","video/3gpp"},
                {".3g2","video/3gpp2"},
                {".7z","application/x-7z-compressed"}
            };

            // 空いてるポートまでループループ
            for (var port = 3000; port < 65535; port++) {
                try {
                    // 実行ディレクトリを指定
                    var pubPath = AppDomain.CurrentDomain.BaseDirectory;

                    HttpListener listener = new HttpListener();
                    listener.Prefixes.Add($"http://localhost:{port}/");
                    listener.Start();
                    Console.WriteLine($"Listening in localhost:{port}");
                    // ブラウザで開かせる
                    System.Diagnostics.Process.Start($"http://localhost:{port}/");

                    for (; ; ) {
                        var context = listener.GetContext();
                        var req = context.Request;
                        var res = context.Response;

                        var wpath = req.RawUrl;
                        Console.WriteLine(wpath);

                        // 何も指定しない、または存在しないものを参照したらindex.html送り(SPA用)
                        if (wpath == "/" || !File.Exists(pubPath + wpath.Replace("/", "\\"))) wpath = "/index.html";

                        string mime = "";
                        extentions.TryGetValue(wpath.Substring(wpath.LastIndexOf(".")),out mime);

                        var path = pubPath + wpath.Replace("/", "\\");

                        try {
                            res.StatusCode = 200;
                            byte[] content = File.ReadAllBytes(path);
                            // 見つからなかったら面倒だから指定しない
                            if(mime != "")
                                res.ContentType = mime;
                            res.OutputStream.Write(content, 0, content.Length);
                        } catch (Exception ex) {
                            // 理論上ここに来ることはないけど(存在しない画像参照しようとしてもindex.html行き食らうから)
                            res.StatusCode = 500;
                            byte[] content = Encoding.UTF8.GetBytes(ex.Message);
                            res.OutputStream.Write(content, 0, content.Length);
                        }
                        res.Close();
                    }
                } catch (HttpListenerException e) {
                    if (e.ErrorCode == 5 || e.ErrorCode == 183) {
                        Console.WriteLine($"{port}/tcp is probably used. Try another port...");
                    } else {
                        Console.WriteLine(e.ErrorCode);
                        return;
                    }
                }
            }

        }

    }
}
