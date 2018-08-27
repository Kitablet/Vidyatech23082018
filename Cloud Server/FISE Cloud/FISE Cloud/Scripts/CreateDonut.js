
function RenderDonutChart(ContainerId,Content, HeaderTitle, SubTitle) {
  
var data={
	"header": {
		"title": {
			"text": HeaderTitle,
			"fontSize": 16,
			"font": "arial",
			"color": "#000",
		},
		"subtitle": {
			"text": SubTitle,
			"color": "#000",
			"fontSize": 12,
			"font": "arial"
		},
		
		"location": "pie-center",
		"titleSubtitlePadding": -10
	},	
	"size": {
	    "canvasWidth": 492,	   
		"pieInnerRadius": "50%",
		"pieOuterRadius": "55%"
	},
	"data": {
		"sortOrder": "value-asc",
		"content":Content
	},
	"labels": {
		"outer": {
			"format": "label-value2",
			"pieDistance": 30
		},
		"inner": {
			"format": "none"
		},
		"mainLabel": {
			"fontSize": 18
		},
		"percentage": {
			"color": "#999999",
			"fontSize": 12,
			"decimalPlaces": 1
		},
		"value": {
			"color": "#000",
			"fontSize": 14
		},
		"lines": {
			"enabled": true,
			"color": "#777777"
		}
	},
	
	"misc": {
		"colors": {
			"segmentStroke": "#FFF"
		}
	},
	"effects": {
		"pullOutSegmentOnClick": {
			"effect": "none"
		}
	}
};
var pie = new d3pie(ContainerId,data );
}