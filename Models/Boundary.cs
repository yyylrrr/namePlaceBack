using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class Boundary
    {
        public string JXDM { get; set; }
        //界桩编号

        [Key]
        public string KJSJBSM { get; set; }
        //界线代码

        public string JXDJ { get; set; }
        //界线等级

        public string JXMC { get; set; }
        //界线名称

        public string JXCD { get; set; }
        //界线长度

        public string JXQD { get; set; }
        //界线起点

        public string JXZD { get; set; }
        //界线止点

        public string JZKS { get; set; }
        //界桩颗数

        public string DLZKS { get; set; }
        //单立桩颗数

        public string SLZKS { get; set; }
        //双立桩颗数

        public string SANLZKS { get; set; }
        //三立桩颗数

        public string QTXX { get; set; }
        //其他信息

        public string 是否争议 { get; set; }
        //是否争议
    }
}