using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class Park
    {
        [Key]
        public string Namecode { get; set; }
        //地名代码

        public string Realname { get; set; }
        //标准名称

        public string Othername { get; set; }
        //别名

        public string Simplename { get; set; }
        //简称

        public string Pastname { get; set; }
        //曾用名

        public string Hanziname { get; set; }
        //汉字书写

        public string Shaoshuname { get; set; }
        //少数名族语言书写

        public string Originpronunce { get; set; }
        //地名原读音

        public string Putongpronunce { get; set; }
        //汉语普通话读音

        public string Pinyin { get; set; }
        //罗马字母拼写

        public string Namelanguage { get; set; }
        //地名语种

        public string Nametype { get; set; }
        //地名类别

        public string Lon { get; set; }
        //东经

        public string Atlon { get; set; }
        //至东经

        public string Lat { get; set; }
        //北纬

        public string Atlat { get; set; }
        //至北纬

        public string Statu { get; set; }
        //地名普查状态

        public string Originname { get; set; }
        //原图名称

        public string Picturecode { get; set; }
        //图号（年版）

        public string Scale { get; set; }
        //比例尺

        public string Usetime { get; set; }
        //使用地名

        public string Nameorigin { get; set; }
        //地名来历
        public string Namemeaning { get; set; }
        //地名来历

        public string Secret { get; set; }
        //密级

        public string Coordinate { get; set; }
        //坐标系

        public string Measurementmethod { get; set; }
        //测量方法

        public string Geographicentity { get; set; }
        //地理实体概况

        public string Mediainformation { get; set; }
        //多媒体信息

        public string Source { get; set; }
        //资料来源

        public string Remark { get; set; }
        //备注

        public string Registertime { get; set; }
        //登记时间

        public string Registerpeople { get; set; }
        //登记人

        public string Registerunit { get; set; }
        //登记单位

        public string Universalpinyin { get; set; }
        //通名罗马字母拼写

        public string Settime { get; set; }
        //设立时间

        public string Abolishtime { get; set; }
        //废止时间

        public string Area { get; set; }
        //占地面积

        public string Manageunit { get; set; }
        //管理单位

        public string Connectadd { get; set; }
        //联系地址

        public string Connecttel { get; set; }
        //管理电话

        public string Mianscene { get; set; }
        //主要景点

    }
}