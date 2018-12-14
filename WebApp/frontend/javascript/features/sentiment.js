var sentiments = {
    1: 'Sad',
    2: 'Frustrated',
    3: 'Satisfied',
    4: 'Excited',
    5: 'Polite',
    6: 'Impolite',
    7: 'Sympathetic'
}
var sentimentData = {}

function getColor(value) {
    //value from 0 to 1
    if (value > 0) {
        var hue = ((1 - value) * 120).toString(10);
        return ["hsl(", hue, ",100%,70%)"].join("");
    }
}

function WatsonTicket(ticketid) {
    window.parent.Ts.Services.WatsonTickets.Ticket(ticketid, function(result) {
        if (result != 'negative' && result != 'nothing' && result != 'hidden') {
            var data = jQuery.parseJSON(result);
            var percentage = data.TicketSentimentScore / 1000;
            var reverse = 1 - percentage;
            var display = parseInt(percentage * 100);
            var color = getColor(reverse);
            $('#health-ticket').css({
                'background-color': color
            }).css({
                'text-align': 'left'
            });
            $('#health-meter').css({
                'width': display + 'px'
            });
            $('#health-message').removeClass('disabled').addClass('enabled').text(display + '%');
        } else {
            var color = getColor(0.100);
        }
    });
    window.parent.Ts.Services.WatsonTickets.Summary(_ticketID, function(r) {
        if (r != 'negative' && r != 'nothing' && r != 'faults') {
            var data = jQuery.parseJSON(r);
            var highest = data.summary[0];
            var emotion = sentiments[highest.SentimentID];
            $('#health-emotions').text(emotion);
        }
    });
}

function WatsonCustomer(organizationID) {
    if (_mainFrame.Ts.System.Organization.UseWatson) {
        window.parent.Ts.Services.Customers.GetOrganizationSentiment(organizationID, function(e) {
            if (e.length > 0) {
                $('#organizationSentiment').show();
                var percentage = e / 1000;
                var reverse = 1 - percentage;
                var display = parseInt(percentage * 100);
                var color = getColor(reverse);
                $('#health-ticket').css({
                    'background-color': color
                }).css({
                    'text-align': 'left'
                });
                $('#health-meter').css({
                    'width': display + 'px'
                });
                $('#health-message').removeClass('disabled').addClass('enabled').text(display + '%');
            }
        });
    } else {
        $("#health-properties").addClass("hide");
    }
}

function WatsonTicketField(ticketid) {
    if (_mainFrame.Ts.System.Organization.UseWatson) {
        window.parent.Ts.Services.WatsonTickets.Ticket(ticketid, function(result) {
            if (result != 'negative' && result != 'nothing' && result != 'hidden') {
                var data = sentimentData = jQuery.parseJSON(result);
                var percentage = data.TicketSentimentScore / 1000;
                var reverse = 1 - percentage;
                var display = parseInt(percentage * 100);
                var output = [];
                output.push(data.TicketSentimentScore + " - ");
                // output.push("<strong>Emotions</strong><br>");
                if (data.Sad) output.push("Sad<br>");
                if (data.Frustrated) output.push("Frustrated<br>");
                if (data.Satisfied) output.push("Satisfied<br>");
                if (data.Excited) output.push("Excited<br>");
                if (data.Polite) output.push("Polite<br>");
                if (data.Impolite) output.push("Impolite<br>");
                if (data.Sympathetic) output.push("Sympathetic<br>");
                $('#ticket-Sentiment').append(output.join(' '));
            }
        });
    } else {
        $("#health-properties").addClass("hide");
    }
}




$(document).on('click', '#ticket-Sentiment', function(e) {
    if (window.parent.Ts.System.User.OrganizationID != 1078) {
        return;
    }

    teamsupport.modals.overlay.show();
    $('#modal').html(Handlebars.templates['chart']).css({
        'width': '740px'
    });
    teamsupport.modals.modal.show('#modal');

    var c = document.getElementById("myCanvas");
    var ctx = c.getContext("2d");

    // BRANCHES, START @ TOP LEFT.

    // BRANCH, TOP LEFT.
    if (sentimentData.Frustrated) {
        ctx.beginPath();
        ctx.moveTo(100, 100);
        ctx.lineTo(200, 200);
        ctx.lineWidth = 5;
        ctx.strokeStyle = "rgba(229,99,66,0.7)";
        ctx.stroke();

        ctx.beginPath();
        ctx.arc(100, 100, 20, 0, 2 * Math.PI);
        ctx.strokeStyle = "rgba(229,99,66,0.5)";
        ctx.lineWidth = 10;
        ctx.stroke();
        ctx.fillStyle = "rgba(229,99,66,1)";
        ctx.fill();

        ctx.beginPath();
        ctx.font = "13px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(0,0,0,0.8)";
        ctx.fillText("Frustrated", 100, 150);

        ctx.beginPath();
        ctx.font = "12px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(229,99,66,1)";
        ctx.fillText(sentimentData.Frustrated, 100, 170);

        ctx.beginPath();
        ctx.font = "20px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(255,255,255,0.8)";
        ctx.fillText("-", 100, 99);
    }

    // BRANCH, TOP LEFT-CENTER.

    if (sentimentData.Impolite) {
        ctx.beginPath();
        ctx.moveTo(250, 100);
        ctx.lineTo(350, 200);
        ctx.lineWidth = 5;
        ctx.strokeStyle = "rgba(237,167,42,0.7)";
        ctx.stroke();

        ctx.beginPath();
        ctx.arc(250, 100, 20, 0, 2 * Math.PI);
        ctx.strokeStyle = "rgba(237,167,42,0.5)";
        ctx.lineWidth = 10;
        ctx.stroke();
        ctx.fillStyle = "rgba(237,167,42,1)";
        ctx.fill();

        ctx.beginPath();
        ctx.font = "13px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(0,0,0,0.8)";
        ctx.fillText("Impolite", 250, 150);

        ctx.beginPath();
        ctx.font = "12px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(237,167,42,1)";
        ctx.fillText(sentimentData.Impolite, 250, 170);

        ctx.beginPath();
        ctx.font = "20px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(255,255,255,0.8)";
        ctx.fillText("-", 250, 99);
    }

    // BRANCH, TOP RIGHT-CENTER.

    if (sentimentData.Polite) {
        ctx.beginPath();
        ctx.moveTo(450, 100);
        ctx.lineTo(350, 200);
        ctx.lineWidth = 5;
        ctx.strokeStyle = "rgba(237,167,42,0.7)";
        ctx.stroke();

        ctx.beginPath();
        ctx.arc(450, 100, 20, 0, 2 * Math.PI);
        ctx.strokeStyle = "rgba(237,167,42,0.5)";
        ctx.lineWidth = 10;
        ctx.stroke();
        ctx.fillStyle = "rgba(237,167,42,1)";
        ctx.fill();

        ctx.beginPath();
        ctx.font = "13px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(0,0,0,0.8)";
        ctx.fillText("Polite", 450, 150);

        ctx.beginPath();
        ctx.font = "12px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(237,167,42,1)";
        ctx.fillText('+' + sentimentData.Polite, 450, 170);

        ctx.beginPath();
        ctx.font = "20px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(255,255,255,0.8)";
        ctx.fillText("+", 450, 99);
    }

    // BRANCH, TOP RIGHT.

    if (sentimentData.Excited) {
        ctx.beginPath();
        ctx.moveTo(500, 200);
        ctx.lineTo(600, 100);
        ctx.lineWidth = 5;
        ctx.strokeStyle = "rgba(122,215,20,0.7)";
        ctx.stroke();

        ctx.beginPath();
        ctx.arc(600, 100, 20, 0, 2 * Math.PI);
        ctx.strokeStyle = "rgba(122,215,20,0.5)";
        ctx.lineWidth = 10;
        ctx.stroke();
        ctx.fillStyle = "rgba(122,215,20,1)";
        ctx.fill();

        ctx.beginPath();
        ctx.font = "13px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(0,0,0,0.8)";
        ctx.fillText("Excited", 600, 150);

        ctx.beginPath();
        ctx.font = "12px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(122,215,20,1)";
        ctx.fillText('+' + sentimentData.Excited, 600, 170);

        ctx.beginPath();
        ctx.font = "20px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(255,255,255,0.8)";
        ctx.fillText("+", 600, 99);
    }

    // BRANCH CIRCLE, BOTTOM RIGHT.

    if (sentimentData.Satisfied) {
        ctx.beginPath();
        ctx.moveTo(500, 200);
        ctx.lineTo(600, 300);
        ctx.lineWidth = 5;
        ctx.strokeStyle = "rgba(122,215,20,0.7)";
        ctx.stroke();

        ctx.beginPath();
        ctx.arc(600, 300, 20, 0, 2 * Math.PI);
        ctx.strokeStyle = "rgba(122,215,20,0.5)";
        ctx.lineWidth = 10;
        ctx.stroke();
        ctx.fillStyle = "rgba(122,215,20,1)";
        ctx.fill();

        ctx.beginPath();
        ctx.font = "13px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(0,0,0,0.8)";
        ctx.fillText("Satisfied", 600, 350);

        ctx.beginPath();
        ctx.font = "12px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(122,215,20,1)";
        ctx.fillText('+' + sentimentData.Satisfied, 600, 370);

        ctx.beginPath();
        ctx.font = "20px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(255,255,255,0.8)";
        ctx.fillText("+", 600, 299);
    }

    // BRANCH CIRCLE, BOTTOM LEFT-CENTER.

    if (sentimentData.Sympathetic) {
        ctx.beginPath();
        ctx.moveTo(250, 300);
        ctx.lineTo(350, 200);
        ctx.lineWidth = 5;
        ctx.strokeStyle = "rgba(237,167,42,0.7)";
        ctx.stroke();

        ctx.beginPath();
        ctx.arc(250, 300, 20, 0, 2 * Math.PI);
        ctx.strokeStyle = "rgba(237,167,42,0.5)";
        ctx.lineWidth = 10;
        ctx.stroke();
        ctx.fillStyle = "rgba(237,167,42,1)";
        ctx.fill();

        ctx.beginPath();
        ctx.font = "13px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(0,0,0,0.8)";
        ctx.fillText("Sympathetic", 250, 350);

        ctx.beginPath();
        ctx.font = "12px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(237,167,42,1)";
        ctx.fillText(sentimentData.Sympathetic, 250, 370);

        ctx.beginPath();
        ctx.font = "20px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(255,255,255,0.8)";
        ctx.fillText("-", 250, 299);
    }

    // BRANCH, BOTTOM LEFT.

    if (sentimentData.Sad) {
        ctx.beginPath();
        ctx.moveTo(100, 300);
        ctx.lineTo(200, 200);
        ctx.lineWidth = 5;
        ctx.strokeStyle = "rgba(229,99,66,0.7)";
        ctx.stroke();

        ctx.beginPath();
        ctx.arc(100, 300, 20, 0, 2 * Math.PI);
        ctx.strokeStyle = "rgba(229,99,66,0.5)";
        ctx.lineWidth = 10;
        ctx.stroke();
        ctx.fillStyle = "rgba(229,99,66,1)";
        ctx.fill();

        ctx.beginPath();
        ctx.font = "13px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(0,0,0,0.8)";
        ctx.fillText("Sad", 100, 350);

        ctx.beginPath();
        ctx.font = "12px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(229,99,66,1)";
        ctx.fillText(sentimentData.Sad, 100, 370);

        ctx.beginPath();
        ctx.font = "20px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(255,255,255,0.8)";
        ctx.fillText("-", 100, 299);
    }

    // LEFT CIRCLE.

    if (sentimentData.Frustrated || sentimentData.Sad) {
        ctx.beginPath();
        ctx.moveTo(200, 200);
        ctx.lineTo(350, 200);
        ctx.lineWidth = 5;
        ctx.strokeStyle = "rgba(229,99,66,0.7)";
        ctx.stroke();

        ctx.beginPath();
        ctx.arc(200, 200, 20, 0, 2 * Math.PI);
        ctx.strokeStyle = "rgba(229,99,66,0.5)";
        ctx.lineWidth = 10;
        ctx.stroke();
        ctx.fillStyle = "rgba(229,99,66,1)";
        ctx.fill();

        ctx.beginPath();
        ctx.font = "20px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(255,255,255,0.8)";
        ctx.fillText("-", 200, 199);
    }

    // RIGHT CIRCLE.

    if (sentimentData.Excited || sentimentData.Satisfied) {
        ctx.beginPath();
        ctx.moveTo(350, 200);
        ctx.lineTo(500, 200);
        ctx.lineWidth = 5;
        ctx.strokeStyle = "rgba(122,215,20,1)";
        ctx.stroke();

        ctx.beginPath();
        ctx.arc(500, 200, 20, 0, 2 * Math.PI);
        ctx.strokeStyle = "rgba(122,215,20,0.5)";
        ctx.lineWidth = 10;
        ctx.stroke();
        ctx.fillStyle = "rgba(122,215,20,1)";
        ctx.fill();

        ctx.beginPath();
        ctx.font = "20px Verdana";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillStyle = "rgba(255,255,255,0.8)";
        ctx.fillText("+", 500, 199);
    }

    // PROGRESS BAR.

    ctx.beginPath();
    ctx.moveTo(100, 400);
    ctx.lineTo(600, 400);
    ctx.lineWidth = 5;
    ctx.strokeStyle = "rgba(0,0,0,0.2)";
    ctx.stroke();

    if (sentimentData.TicketSentimentScore > 50) {
        var distance = (sentimentData.TicketSentimentScore - 50) * 5 + 350;
        var primaryColor = "rgba(122,215,20,0.9)";
        ctx.beginPath();
        ctx.moveTo(350, 400);
        ctx.lineTo(distance, 400);
        ctx.lineWidth = 5;
        ctx.strokeStyle = primaryColor;
        ctx.stroke();
    } else if (sentimentData.TicketSentimentScore < 50) {
        var distance = 350 - (50 - sentimentData.TicketSentimentScore) * 5;
        var primaryColor = "rgba(229,99,66,0.9)";
        ctx.beginPath();
        ctx.moveTo(350, 400);
        ctx.lineTo(distance, 400);
        ctx.lineWidth = 5;
        ctx.strokeStyle = primaryColor;
        ctx.stroke();
    }

    ctx.beginPath();
    ctx.moveTo(348, 400);
    ctx.lineTo(352, 400);
    ctx.lineWidth = 5;
    ctx.strokeStyle = "rgba(0,0,0,0.6)";
    ctx.stroke();

    // CENTER CIRCLE.

    ctx.beginPath();
    ctx.arc(350, 200, 50, 0, 2 * Math.PI);
    ctx.strokeStyle = "rgba(122,215,20,0.5)";
    ctx.lineWidth = 20;
    ctx.stroke();
    ctx.fillStyle = "#7ad714";
    ctx.fill();

    ctx.beginPath();
    ctx.font = "20px Verdana";
    ctx.textAlign = "center";
    ctx.textBaseline = "middle";
    ctx.fillStyle = "rgba(0,0,0,0.8)";
    ctx.fillText("Sentiment", 350, 290);

    ctx.beginPath();
    ctx.font = "16px Verdana";
    ctx.textAlign = "center";
    ctx.textBaseline = "middle";
    ctx.fillStyle = primaryColor;
    ctx.fillText(sentimentData.TicketSentimentScore + " / 100", 350, 315);

});
