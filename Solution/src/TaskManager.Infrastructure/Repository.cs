﻿using Infrastructure;

namespace TaskManager.Infrastructure
{
    public class Repository<T> : Repository<T, ApplicationDbContext> where T : class
    {
        public Repository(ApplicationDbContext context) : base(context)
        {
        }
    }
}