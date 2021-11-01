using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CopyToLocales.Postgressql.Dto
{
    [Table("localizations")]
    public class LocalizationStringModel
    {
        [Column("key")]
        [Key]
        public string Key { get; set; }
        
        [Column("ar_sa")]
        public string Arabic { get; set; }
        
        [Column("cs_cz")]
        public string Czech { get; set; }
        
        [Column("de_DE")]
        public string German { get; set; }
        
        [Column("el_GR")]
        public string Greek { get; set; }
        
        [Column("en_US")]
        public string English_US { get; set; }
        
        [Column("es_ES")]
        public string Spanish { get; set; }
        
        [Column("fr_FR")]
        public string French { get; set; }
        
        [Column("hu_HU")]
        public string Hungarian { get; set; }
        
        [Column("it_IT")]
        public string Italian { get; set; }
        
        [Column("ja_JP")]
        public string Japanese { get; set; }
        
        [Column("ko_KR")]
        public string South_Korean { get; set; }
        
        [Column("nl_NL")]
        public string Dutch { get; set; }
        
        [Column("pl_PL")]
        public string Polish { get; set; }
        
        [Column("pt_BR")]
        public string Brazilian_Portuguese { get; set; }
        
        [Column("ru_RU")]
        public string Russian { get; set; }
        
        [Column("sk_SK")]
        public string Slovak { get; set; }
        
        [Column("sr_Latn_RS")]
        public string Serbian { get; set; }
        
        [Column("tr_TR")]
        public string Turkish { get; set; }
        
        [Column("zh_HK")]
        public string Traditional_Chinese { get; set; }
        
        [Column("zh_CN")]
        public string Simplified_Chinese { get; set; }
        
        [Column("uk_UA")]
        public string Ukrainian { get; set; }
    }
}