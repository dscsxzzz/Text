using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GenericServices.Configuration;
using Microsoft.AspNetCore.JsonPatch.Internal;
using SharedModels.Dtos;
using SharedModels.Models;

namespace SharedModels.Mappings;

public class UserReadSimpleDtoMapping : PerDtoConfig<UserReadSimpleDto, User>
{
    public sealed override Action<IMappingExpression<User, UserReadSimpleDto>> AlterReadMapping
    {
        get
        {
            return cfg => cfg
                .ForMember(dto => dto.Username,
                    feat => feat.MapFrom(entity => entity.Username));
        }
    }
}

