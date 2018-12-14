/// <reference path="ts/ts.js" />
/// <reference path="ts/parent.parent.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />

var adminPortal = null;
$(document).ready(function () {
  adminPortal = new AdminPortal();
  adminPortal.refresh();
});

function onShow() {
  adminPortal.refresh();
};


AdminPortal = function () {
  $('#btnRefresh')
  .click(function (e) {
    e.preventDefault();
    window.location = window.location;
  })
  .toggle(window.location.hostname.indexOf('127.0.0.1') > -1);

    var height = $('.portal-admin').height();
  $('iframe').css('height', height * 0.85 | 0);

  $(function () {
      $(".slider").slider({
          range: "min",
          value: 2,
          min: 0,
          max: 10,
          step: .5,
          slide: function (event, ui) {
              $(this).next().text("Overall Weight: " + (ui.value*10) + "%");
          },
          stop: function (event, ui) {
              var total = 0;
              var result;

              $('.slider').each(function () {
                  total = total + $(this).slider("value");
              });

              if (total == 10)
              {
                  result = "100%";
                  $('#cdi-total').removeClass('red');
                  $('#recalculate-cdi').removeAttr("disabled");
                  $('.portal-save-panel').show();
              }
                  
              if (total > 10)
              {
                  result = (total * 10) + "% is greater than 100%, please reconfigure your weights";
                  $('#cdi-total').addClass('red');
                  $('#recalculate-cdi').attr("disabled", "disabled");
                  $('.portal-save-panel').hide();
              }
                  
              if (total < 10)
              {
                  result = (total * 10) + "% is less than 100%, please reconfigure your weights";
                  $('#cdi-total').addClass('red');
                  $('#recalculate-cdi').attr("disabled", "disabled");
                  $('.portal-save-panel').hide();
              }

              $('#cdi-total').text("Total Weight: " + result);
              
          }
      });

      $("#cdi-green").slider({
          range: "min",
          value: 70,
          min: 0,
          max: 100,
          slide: function (event, ui) {
              $(this).next().text("Upper Limit: " + (ui.value));
          },
          stop: function (event,ui)
          {
              if (ui.value > $("#cdi-yellow").slider('value'))
              {
                  $(this).next().addClass("red");
                  $('.portal-save-panel').hide();
              }
              else{
                  $("#cdi-green").next().removeClass("red");
                  $('.portal-save-panel').show();
              }
          }
      });

      $("#cdi-yellow").slider({
          range: "min",
          value: 85,
          min: 0,
          max: 100,
          slide: function (event, ui) {
              $(this).next().text("Upper Limit: " + (ui.value));
          },
          stop: function (event,ui)
          {
              if (ui.value < $("#cdi-green").slider("value"))
              {
                  $(this).next().addClass("red");
                  $('.portal-save-panel').hide();
              }
              else {
                  $("#cdi-yellow").next().removeClass("red");
                  $(this).next().removeClass("red");
                  $('.portal-save-panel').show();
              }
          }
      });
  });


  //$('a').addClass('ui-state-default ts-link');
  $('button').button();
  $('#tabs').tabs().find('.ui-tabs-panel').addClass('ui-corner-all');
  $('.loading-section').hide().next().show();

  $('.tabs-modifiable input.text, .tabs-modifiable input.checkbox, .tabs-modifiable textarea').click(function (e) {
    $('.portal-save-panel').show();
  });

  $('.portal-save-panel').hide();
  $('.portal-save').click(function (e) { saveValues();  });
  $('.portal-cancel').click(function (e) {
    e.preventDefault();
    $('.portal-save-panel').hide();
    getData();
  });



  function saveValues() {

    var _cdiOption = new parent.parent.TeamSupport.Data.CDI_SettingProxy();
    _cdiOption.TotalTicketsWeight = ($('#ttw-slider').slider('value') * .1);
    _cdiOption.OpenTicketsWeight = $('#otw-slider').slider('value') * .1;
    _cdiOption.Last30Weight = $('#last30-slider').slider('value') * .1;
    _cdiOption.AvgDaysOpenWeight = $('#avgopen-weight').slider('value') * .1;
    _cdiOption.AvgDaysToCloseWeight = $('#avgclose-weight').slider('value') * .1;
    _cdiOption.AverageActionCountWeight = $('#actionCount-weight').slider('value') * .1;
    _cdiOption.AverageSentimentScoreWeight = $('#ticketSentiment-weight').slider('value') * .1;
    _cdiOption.AverageSeverityWeight = $('#ticketSeverity-weight').slider('value') * .1;
    _cdiOption.AgentRatingsWeight = $('#agentRatings-weight').slider('value') * .1;
    _cdiOption.GreenUpperRange = $("#cdi-green").slider('value');
    _cdiOption.YellowUpperRange = $("#cdi-yellow").slider('value');

    parent.parent.Ts.Services.Organizations.SaveCDISettings(_cdiOption, function (result) {
        parent.parent.Ts.System.logAction('Admin Company - CDI Settings Saved');
        $('.portal-save-panel').hide();
    });


    var _agentratingOption = new parent.parent.TeamSupport.Data.AgentRatingsOptionProxy();;
    _agentratingOption.PositiveRatingText = $('#agentrating-positive').val();
    _agentratingOption.NeutralRatingText = $('#agentrating-neutral').val();
    _agentratingOption.NegativeRatingText = $('#agentrating-negative').val();
    _agentratingOption.RedirectURL = $('#agentrating-redirecturl').val();
    _agentratingOption.ExternalPageLink = $('#agentrating-externalurl').val();

    parent.parent.Ts.Services.Organizations.SaveAgentRatings(_agentratingOption, function (result) {
        parent.parent.Ts.System.logAction('Admin Company - Agent Rating Settings Saved');
        $('.portal-save-panel').hide();
    });

  }

  loadAgentRating();
  function loadAgentRating() {

      parent.parent.Ts.Services.Organizations.GetAgentRatingOptions(parent.parent.Ts.System.Organization.OrganizationID, function (o) {
          if (o != null) {
              if (o.PositiveImage)
                  $('#agentrating-positive-img').attr('src', o.PositiveImage);
              if (o.NeutralImage)
                  $('#agentrating-neutral-img').attr('src', o.NeutralImage);
              if (o.NegativeImage)
                  $('#agentrating-negative-img').attr('src', o.NegativeImage);

              if (o.PositiveRatingText != null)
                  $('#agentrating-positive').text(o.PositiveRatingText);
              if (o.NeutralRatingText != null)
                  $('#agentrating-neutral').text(o.NeutralRatingText);
              if (o.NegativeRatingText != null)
                  $('#agentrating-negative').text(o.NegativeRatingText);
              if (o.RedirectURL != null)
                  $('#agentrating-redirecturl').val(o.RedirectURL);
              if (o.ExternalPageLink != null)
                  $('#agentrating-externalurl').val(o.ExternalPageLink);
          }
      });
  }

  loadCDISettings();
  function loadCDISettings() {
      parent.parent.Ts.Services.Organizations.LoadCDISettings(parent.parent.Ts.System.Organization.OrganizationID, function (cdi) {

          if (cdi != null)
            {
              var ttwvalue;
              var last30slider;
              var otwslider;
              var avgopenweight;
              var avgcloseweight;
              var ticketSentimentweight;
              var actionCountweight;
              var ticketSeverityweight;
              var agentRatingsWeight;

              // if original five contain a NULL - use defaults
              var useDefaults = (cdi.TotalTicketsWeight == null) || (cdi.Last30Weight == null) ||
                  (cdi.OpenTicketsWeight == null) || (cdi.AvgDaysOpenWeight == null) || (cdi.AvgDaysToCloseWeight == null);
              if (useDefaults) {
                  ttwvalue = '2.0';
                  last30slider = '2.0';
                  otwslider = '2.0';
                  avgopenweight = '2.0';
                  avgcloseweight = '2.0';
                  ticketSentimentweight = '0';
                  actionCountweight = '0';
                  ticketSeverityweight = '0';
                  agentRatingsWeight = '0';
              }
              else {
                  // use what is configured
                  ttwvalue = cdi.TotalTicketsWeight == null ? '0' : cdi.TotalTicketsWeight * 10;
                  last30slider = cdi.Last30Weight == null ? '0' : cdi.Last30Weight * 10;
                  otwslider = cdi.OpenTicketsWeight == null ? '0' : cdi.OpenTicketsWeight * 10;
                  avgopenweight = cdi.AvgDaysOpenWeight == null ? '0' : cdi.AvgDaysOpenWeight * 10;
                  avgcloseweight = cdi.AvgDaysToCloseWeight == null ? '0' : cdi.AvgDaysToCloseWeight * 10;
                  ticketSentimentweight = cdi.AverageSentimentScoreWeight == null ? '0' : cdi.AverageSentimentScoreWeight * 10;
                  actionCountweight = cdi.AverageActionCountWeight == null ? '0' : cdi.AverageActionCountWeight * 10;
                  ticketSeverityweight = cdi.AverageSeverityWeight == null ? '0' : cdi.AverageSeverityWeight * 10;
                  agentRatingsWeight = cdi.AgentRatingsWeight == null ? '0' : cdi.AgentRatingsWeight * 10;
              }

              $('#ttw-slider').slider('value', ttwvalue);
              $('#ttw-slider').next().text("Overall Weight: " + (ttwvalue * 10) + "%");

              $('#last30-slider').slider('value', last30slider);
              $('#last30-slider').next().text("Overall Weight: " + (last30slider * 10) + "%");

              $('#otw-slider').slider('value', otwslider);
              $('#otw-slider').next().text("Overall Weight: " + (otwslider * 10) + "%");

              $('#avgopen-weight').slider('value', avgopenweight);
              $('#avgopen-weight').next().text("Overall Weight: " + (avgopenweight * 10) + "%");

              $('#avgclose-weight').slider('value', avgcloseweight);
              $('#avgclose-weight').next().text("Overall Weight: " + (avgcloseweight * 10) + "%");

              $('#ticketSentiment-weight').slider('value', ticketSentimentweight);
              $('#ticketSentiment-weight').next().text("Overall Weight: " + (ticketSentimentweight * 10) + "%");

              $('#actionCount-weight').slider('value', actionCountweight);
              $('#actionCount-weight').next().text("Overall Weight: " + (actionCountweight * 10) + "%");

              $('#ticketSeverity-weight').slider('value', ticketSeverityweight);
              $('#ticketSeverity-weight').next().text("Overall Weight: " + (ticketSeverityweight * 10) + "%");

              $('#agentRatings-weight').slider('value', agentRatingsWeight);
              $('#agentRatings-weight').next().text("Overall Weight: " + (agentRatingsWeight * 10) + "%");

              var greenlimit = cdi.GreenUpperRange == null ? '70' : cdi.GreenUpperRange;
              $('#cdi-green').slider('value', greenlimit);
              $('#cdi-green').next().text("Upper Limit: " + greenlimit);

              var yellowlimit = cdi.YellowUpperRange == null ? '85' : cdi.YellowUpperRange;
              $('#cdi-yellow').slider('value', yellowlimit);
              $('#cdi-yellow').next().text("Upper Limit: " + yellowlimit);

              var lastCompute = cdi.LastCompute == null ? 'Never' : parent.parent.Ts.Utils.getMsDate(cdi.LastCompute);

              if (lastCompute == 'Never') {
                  var cdistatus = 'Never';
              }
              else {
                  var cdistatus = lastCompute.localeFormat(parent.parent.Ts.Utils.getDateTimePattern())
              }

              $('#cdiStatus').html("The CDI runs once per day, and the last time your account processed was: <strong>" + cdistatus + "</strong> To force an update now, please click the force update button below.");
          }
      });

  }

  //var V2OrgID = parent.parent.Ts.System.User.OrganizationID;
  //if (!(V2OrgID === 1078 || V2OrgID === 1088 || V2OrgID === 13679 || V2OrgID === 362372)) { $('.tabs-order').remove(); }

  $('#recalculate-cdi').click(function () {
      parent.parent.Ts.Services.Organizations.ResetCDI();
      alert("TeamSupport will begin recomputing the CDI indexes momentarily - Please note that it could take as much as 30 minutes for this process to be completed.");
  });

  $('.tabs-cdi').click(function () {
      loadCDISettings();

      $("#cdi-green").next().removeClass("red");
      $("#cdi-yellow").next().removeClass("red");
      $('#cdi-total').removeClass('red');
      $('#recalculate-cdi').removeAttr("disabled");
      $('.portal-save-panel').hide();
      $('#cdi-total').text("Total Weight: 100%");

  });
};


AdminPortal.prototype = {
  constructor: AdminPortal,
  refresh: function () {

  }
};
