// 城市信息处理相关
var city = define(['jquery', "helper"], function ($, helper) {
    var _shortName = null;
    var _name = null;
    var _allCityInfo = [];

    var initAllCityInfo = function() {
        $.getJSON("DomainJS/city.json", function(data) {
            _allCityInfo = data;
        });
    }

    var shortCut = function() {
        var filterarray = $.grep(_allCityInfo, function(obj) {
            return obj.cityName == _name;
        });
        _shortName = filterarray instanceof Array ?
            (filterarray.length > 0 ? filterarray[0].shortCut : "") : filterarray != null ? filterarray.shortCut : "";
    }

    return {
        get shortName() {
            return _shortName;
        },
        set shortName(value) {
            _shortName = value;
        },
        get name() {
            if (dataResource == "douban") {
                _name = helper.getQueryString("cityname");
            } else if (dataResource == "huzhuzufang")
            {
                _name = "上海";
            }
            return _name;
        },
        set name(value) {
            _name = value;
        },
        initAllCityInfo: initAllCityInfo,
        shortCut: shortCut
    }
});