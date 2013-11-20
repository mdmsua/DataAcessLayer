﻿using DataAccessLayer.Core;

namespace DataAccessLayer.Services
{
    sealed class Scalar : IScalar
    {
        readonly IDbCore _dbCore;

        public Scalar(IDbCore dbCore)
        {
            _dbCore = dbCore;
        }
        
        public T Execute<T>(string procedure, DbParameters parameters)
        {
            return _dbCore.ExecuteScalar<T>(procedure, parameters);
        }
    }
}
