using System;
using System.Collections.Generic;
using Covid19App.Models;

namespace Covid19App.Services
{
    public interface IItemRepository
    {
        void Add(Item item);
        void Update(Item item);
        Item Remove(string key);
        Item Get(string id);
        IEnumerable<Item> GetAll();
    }
}
