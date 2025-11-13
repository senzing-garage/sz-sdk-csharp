namespace Senzing.Sdk.Tests.Util;

using System.Runtime.CompilerServices;

internal sealed class AccessToken : Object
{
    /// <summary>
    /// Public default constructor.
    /// </summary>
    public AccessToken() { }


    /// <summary>
    /// Equality implemented to perform referential equality.
    /// </summary>
    /// 
    /// <param name="obj">The object to compare with.</param>
    /// 
    /// <returns><c>true</c> if referentially equal, otherwise <c>false</c>.</returns>
    public override sealed bool Equals(object? obj)
    {
        return (this == obj);
    }

    /// <summary>
    /// Implemented to produce a hash code consistent with referential equality. 
    /// </summary>
    /// 
    /// <returns>
    /// A hash code consistent with referential equality.
    /// </returns>
    public override sealed int GetHashCode()
    {
        return RuntimeHelpers.GetHashCode(this);
    }

    /// <summary>
    /// The thread-local instance to make available via <see cref="GetThreadAccessToken"/> 
    /// and <see cref="ClaimThreadAccessToken"/>. 
    /// </summary>
    /// 
    /// <remarks>
    /// The value stored is changed by calls to <see cref="ClaimThreadAccessToken"/>
    /// </remarks>
    private static readonly ThreadLocal<AccessToken> ThreadAccessToken
        = new ThreadLocal<AccessToken>();

    /// <summary>
    /// Returns the current thread-local <c>AccessToken</c> to use for
    /// granting privileged access to restricted methods from objects.
    /// </summary>
    /// 
    /// <remarks>
    /// The return value from this method will be the same <b>until</b>
    /// the <c>AccessToken</c> is claimed by <see cref="claimThreadAccessToken"/>.
    /// </remarks>
    /// 
    /// <returns>
    /// The current thread-local <c>AccessToken</c> for this thread which
    /// will change once <see cref="claimThreadAccessToken"/> is called.
    /// </returns>
    public static AccessToken GetThreadAccessToken()
    {
        AccessToken? token = ThreadAccessToken.Value;
        if (token == null)
        {
            token = new AccessToken();
            ThreadAccessToken.Value = token;
        }
        return token;
    }

    /// <summary>
    /// Claims the <see cref="getThreadAccessToken">current thread-local
    /// access token</see> and forces it to be changed so that subsequent
    /// calls to <see cref="getThreadAccessToken"/> will not return the
    /// same value.
    /// </summary>
    ///
    /// <returns>
    /// The previous thread-local <c>AccessToken</c> after changing the
    /// current thread-local <c>AccessToken</c>.
    /// </returns>
    public static AccessToken ClaimThreadAccessToken()
    {
        AccessToken? token = AccessToken.GetThreadAccessToken();
        ThreadAccessToken.Value = new AccessToken();
        return token;
    }
}
