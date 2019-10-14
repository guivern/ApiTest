using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ApiTest.Models;

namespace ApiTest.Helpers
{
    public static class OrderByExpressions
    {
        public static IDictionary<string, Expression<Func<Pais, object>>> PaisesOrderBy = new Dictionary<string, Expression<Func<Pais, object>>>()
        {
            {"id", p => p.Id},
            {"nombre", p => p.Nombre},
            {"sigla", p => p.Sigla},
        };
        
        public static IDictionary<string, Expression<Func<Ciudad, object>>> CiudadesOrderBy = new Dictionary<string, Expression<Func<Ciudad, object>>>()
        {
            {"id", c => c.Id},
            {"nombre", c => c.Nombre},
            {"escapital", c => c.EsCapital},
            {"paisnombre", c => c.Pais.Nombre},
        };
    }
}