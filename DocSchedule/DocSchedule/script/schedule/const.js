
var _testDat =
[
    {
        Row: 0,
        Data:
            [
                {
                    Serial: "",
                    ID: "1",
                    DocID: "1",
                    Date: "20180322",
                    Style: "left:30px;top:0px;width:300px",
                    Stime: '9:00',
                    Etime: '14:00',
                    Update: '2018/03/21 松田',
                    Type: 0
                },
                {
                    Serial: "",
                    ID: "2",
                    DocID: "2",
                    Date: "20180322",
                    Style: "left:390px;top:0px;width:300px",
                    Stime: '15:00',
                    Etime: '20:00',
                    Update: '2018/03/21 松田',
                    Type: 0
                },
            ]
    },
    {
        Row: 1,
        Data:
            [
                {
                    Serial: "",
                    ID: "3",
                    DocID: "2",
                    Date: "20180323",
                    Style: "left:90px;top:0px;width:120px",
                    Stime: '9:00',
                    Etime: '12:00',
                    Update: '2018/03/21 松田',
                    Type: 1
                }
            ]
    },
    {
        Row: 2,
        Data:
            [
                {
                    Serial: "",
                    ID: "4",
                    DocID: "3",
                    Date: "20180323",
                    Style: "left:30px;top:0px;width:300px",
                    Stime: '10:00',
                    Etime: '15:00',
                    Update: '2018/03/21 松田',
                    Type: 0,
                    SubType:0
                }
            ]
    }
]

var _testDoc = [
    {
        DocID: "1",
        DocName: "テスト医師",
        DocName_R: "医師A",
        DocName_H: "てすといし",
        Comment: "頭",
        Count: "14",
        Speed: "早",
        Main: "2",
        Body:{
            OK: [{ Id: 0, Text: "aaa" }, { Id: 1, Text: "vvv" }, { Id: 2, Text: "asss" }],
            NG: [{ Id: 4, Text: "aaa" }],
            Other: []
        },
        Hosp: {
            OK: ["b", "a", "v"],
            NG: [],
            Other: ["aaaa"]
        },
        Element: "属性",
        Memo: "aaaaaaaaaaaaaaaaaaaaaaa",
        Color: "#9FEDB1",
        Color2: "black"
}, {
        DocID: "2",
        DocName: "テストDr",
        DocName_R: "医師B",
        DocName_H: "てすといし",
        Comment: "71",
        Count: "14",
        Speed: "早",
        Main: "1",
        Body: {
            OK: [{ Id: 0, Text: "aaa" }, { Id: 1, Text: "vvv" }, { Id: 2, Text: "asss" }],
            NG: [{ Id: 4, Text: "aaa" }],
            Other: []
        },
        Hosp: {
            OK: ["aaa", "vvv" , "asss" ],
            NG: ["aaa"],
            Other: []
        },
        Element: "属性",
        Memo: "aaaaaaaaaaaaaaaaaaaaaaa",
        Color: "#FF9761",
        Color2: "black"
    }, {
        DocID: "3",
        DocName: "テストDr2",
        DocName_R: "医師C",
        DocName_H: "てすといし",
        Comment: "テストです",
        Count: "14",
        Speed: "早",
        Main: "0",
        Body: {
            OK: [{ Id: 0, Text: "aaa" }, { Id: 1, Text: "vvv" }, { Id: 2, Text: "asss" }],
            NG: [{ Id: 4, Text: "aaa" }],
            Other: [{ Id: 5, Text: "bbbbb" }]
        },
        Hosp: {
            OK: ["b", "a", "v"],
            NG: [],
            Other: ["aaaa"]
        },
        Element: "属性",
        Memo: "aaaaaaaaaaaaaaaaaaaaaaa",
        Color: "#B200FF",
        Color2: "black"
    }
]

var _docSch = [
    {
        Sdate: '',
        Edate: '',
        Schedule: [
            //{
            //    Day: 1,
            //    Type: 0,
            //    Count: '',
            //    Stime: 1,
            //    Etime: 20,
            //}, {
            //    Day: 2,
            //    Type: 0,
            //    Count: '',
            //    Stime: 6,
            //    Etime: 10,
            //}, {
            //    Day: 5,
            //    Type: 0,
            //    Count: '',
            //    Stime: 6,
            //    Etime: 20,
            //}
        ]
    }
]

var _ColorList = [
]

var BodyPartList = [
    {
        Id: 0,
        Text: 'もりした頭部テンプレ'
    }, {
        Id: 1,
        Text: '心臓ＣＴ'
    }, {
        Id: 2,
        Text: '乳腺ＭＲ'
    }, {
        Id: 3,
        Text: 'MG'
    }, {
        Id: 4,
        Text: 'AI'
    }, {
        Id: 5,
        Text: 'ハイメディックPT'
    }
]

var TimeList = [
     { key: 0, value: '8:30' }
, { key: 1, value: '9:00' }
, { key: 2, value: '9:15' }
, { key: 3, value: '9:30' }
, { key: 4, value: '9:45' }
, { key: 5, value: '10:00' }
, { key: 6, value: '10:15' }
, { key: 7, value: '10:30' }
, { key: 8, value: '10:45' }
, { key: 9, value: '11:00' }
, { key: 10, value: '11:15' }
, { key: 11, value: '11:30' }
, { key: 12, value: '11:45' }
, { key: 13, value: '12:00' }
, { key: 14, value: '12:15' }
, { key: 15, value: '12:30' }
, { key: 16, value: '12:45' }
, { key: 17, value: '13:00' }
, { key: 18, value: '13:15' }
, { key: 19, value: '13:30' }
, { key: 20, value: '13:45' }
, { key: 21, value: '14:00' }
, { key: 22, value: '14:15' }
, { key: 23, value: '14:30' }
, { key: 24, value: '14:45' }
, { key: 25, value: '15:00' }
, { key: 26, value: '15:15' }
, { key: 27, value: '15:30' }
, { key: 28, value: '15:45' }
, { key: 29, value: '16:00' }
, { key: 30, value: '16:15' }
, { key: 31, value: '16:30' }
, { key: 32, value: '16:45' }
, { key: 33, value: '17:00' }
, { key: 34, value: '17:15' }
, { key: 35, value: '17:30' }
, { key: 36, value: '17:45' }
, { key: 37, value: '18:00' }
, { key: 38, value: '18:15' }
, { key: 39, value: '18:30' }
, { key: 40, value: '18:45' }
, { key: 41, value: '19:00' }
, { key: 42, value: '19:15' }
, { key: 43, value: '19:30' }
, { key: 44, value: '19:45' }
, { key: 45, value: '20:00' }
, { key: 46, value: '20:00-' }
]

