using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class BoundaryStake
    {
        public string JZBH { get; set; }
        //界桩编号

        [Key]
        public string KJSJBSM { get; set; }
        //空间数据标识码

        public string JXDM { get; set; }
        //界线代码

        public double ZZB { get; set; }
        //纵坐标

        public double HZB { get; set; }
        //横坐标

        public string JD { get; set; }
        //经度

        public string WD { get; set; }
        //纬度

        public string JZDJ { get; set; }
        //界桩等级

        public string JZLX { get; set; }
        //界桩类型

        public string JZCZ { get; set; }
        //界桩材质

        public string SZD1 { get; set; }
        //所在地1

        public float GC { get; set; }
        //高程

        public string YHFWWWZ { get; set; }
        //一号方位物位置

        public string YHFWWFWJ { get; set; }
        //一号方位物方位角

        public string YHFWWJL { get; set; }
        //一号方位物距离

        public string EHFWWWZ { get; set; }
        //二号方位物位置

        public string EHFWWFWJ { get; set; }
        //二号方位物方位角

        public string EHFWWJL { get; set; }
        //二号方位物距离

        public string SHFWWWZ { get; set; }
        //三号方位物位置

        public string SHFWWFWJ { get; set; }
        //三号方位物方位角

        public string SHFWWJL { get; set; }
        //三号方位物距离

        public string WZMS { get; set; }
        //位置描述
    }
}
