using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JWTCommonLibForDotNetCore.Entities;

namespace JWTCommonLibForDotNetCore.Database
{
    public class IdentityManager : IDataRepository<Identity>
    {
        readonly IdentityContext _identityContext;

        public IdentityManager(IdentityContext context)
        {
            _identityContext = context;
        }

        public IEnumerable<Identity> GetAll()
        {
            return _identityContext.Identities.ToList();
        }

        public ICollection<TType> Get<TType>(Expression<Func<Identity, bool>> where, Expression<Func<Identity, TType>> select) where TType : class
        {
            return _identityContext.Identities.Where(where).Select(select).ToList();
        }

        public Identity Get(Guid id)
        {
            return _identityContext.Identities.FirstOrDefault(e => e.Id == id);
        }

        public void Add(Identity entity)
        {
            _identityContext.Identities.Add(entity);
            _identityContext.SaveChanges();
        }

        public void Update(Identity identity, Identity entity)
        {
            identity.Password = entity.Password;
            identity.Roles = entity.Roles;
            identity.Username = entity.Username;
            _identityContext.SaveChanges();
        }

        public void Delete(Identity identity)
        {
            _identityContext.Identities.Remove(identity);
            _identityContext.SaveChanges();
        }
    }
}