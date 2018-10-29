using System;
using System.Web;

namespace SoftwareTokenProvider
{
    public static class Extensions
    {
        public static AuthData ToAuthData(this string url)
        {
            var uri = new Uri(url);
            var queryString = HttpUtility.ParseQueryString(uri.Query);
            var appDetails = uri.Segments[1];

            return new AuthData
            {
                Scheme = uri.Scheme,
                Host = uri.Host,

                Application = HttpUtility.UrlDecode(appDetails.Split(':')[0]),
                User = HttpUtility.UrlDecode(appDetails.Split(':')[1]),

                Issuer = queryString.Get("issuer"),
                Secret = queryString.Get("secret"),
                Digits = int.Parse(queryString.Get("digits"))
            };
        }
    }
}