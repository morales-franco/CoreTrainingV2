using AutoMapper;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.Data
{
    public class DutchMappingProfile : Profile
    {
        public DutchMappingProfile()
        {
            CreateMap<Order, OrderVM>()
                .ForMember(d => d.OrderId, opt => opt.MapFrom(s => s.Id))
                .ReverseMap();

            /*
             * En este caso podemos decirle que cree un Mapper reverso ReverseMap();
             * Esto crearia el siguiente mapper:
                CreateMap<OrderVM, Order>()
               .ForMember(d => d.Id, opt => opt.MapFrom(s => s.OrderId));
               Es decir un mapper reverso del existente ahorrandonos estas lineas de código
               */
        }
    }
}
