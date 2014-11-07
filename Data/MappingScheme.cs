using System;
using System.Text;
using System.Threading.Tasks;
using AcklenAvenue.Data;
using Domain.Entities;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions.Helpers;

namespace Data
{
    public class MappingScheme : IDatabaseMappingScheme<MappingConfiguration>
    {
        public Action<MappingConfiguration> Mappings
        {
            get
            {
                var autoPersistenceModel = AutoMap.Assemblies(typeof(IEntity).Assembly)
                    .Where(t => typeof(IEntity).IsAssignableFrom(t))
                    // .UseOverridesFromAssemblyOf<AccountOverride>()
                    .Conventions.Add(DefaultCascade.All());

                return x => x.AutoMappings.Add(autoPersistenceModel);
            }
        }
    }
}
