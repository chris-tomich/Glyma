﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace TransactionalNodeService.Model
{
    public interface IMapElementLoader
    {
        void LoadElement(IDataRecord record);
    }
}