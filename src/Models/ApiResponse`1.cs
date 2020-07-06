using System;
using System.Collections.Generic;
using System.Text;

namespace FocusMark.App.Cli.Models
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public string[] Errors { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
