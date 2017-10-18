﻿using System;

namespace TrustchainCore.Interfaces
{
    /// <summary>
    /// http://www.dotnet-programming.com/post/2017/05/08/Aspnet-core-Deserializing-Json-with-Dependency-Injection.aspx
    /// </summary>
    public interface IDIReverseMeta
    {
        bool IsRegistred(Type t);
        Type RegistredTypeFor(Type t);
    }
}
