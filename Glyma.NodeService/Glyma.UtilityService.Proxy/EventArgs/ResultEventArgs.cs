﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using TransactionalNodeService.Proxy;

namespace Glyma.UtilityService.Proxy
{
    public class ResultEventArgs<T> : EventRegisterEventArgs, IResult<T>
    {
        public ResultEventArgs()
            : base()
        {
        }

        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public T Result { get; set; }
    }
}
