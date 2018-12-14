(function ($) {
  $.widget("ui.combobox", {
    _create: function () {
      var input,
					that = this,
					select = this.element.hide(),
					selected = select.children(":selected"),
					value = selected.val() ? selected.text() : "",
					wrapper = this.wrapper = $("<span>")
						.addClass("ui-combobox")
						.insertAfter(select);

      function removeIfInvalid(element) {
        var value = $(element).val(),
						matcher = new RegExp("^" + $.ui.autocomplete.escapeRegex(value) + "$", "i"),
						valid = false;
        select.children("option").each(function () {
          if ($(this).text().match(matcher)) {
            this.selected = valid = true;
            return false;
          }
        });
        if (!valid) {
          // remove invalid value, as it didn't match anything
          $(element)
							.val("")
							.attr("title", value + " didn't match any item")
							.tooltip("open");
          select.val("");
          setTimeout(function () {
            input.tooltip("close").attr("title", "");
          }, 2500);
          input.data("autocomplete").term = "";
          return false;
        }
      }

      input = this.input = $("<input>")
					.appendTo(wrapper)
					.val(value)
          .width(select.width() < 50 ? "100px" : select.width()+20)
          //.width(select.width() < 50 ? "100px" : select.width())
					.attr("title", "")
					.addClass("ui-combobox-input")
					.autocomplete({
					  delay: 0,
					  minLength: 0,
					  position: { collision: "flip" },
					  source: function (request, response) {
					    var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
					    response(select.children("option").map(function () {
					      var text = $(this).text();
					      if (this.value && (!request.term || matcher.test(text)))
					        return {
					          label: text.replace(
											new RegExp(
												"(?![^&;]+;)(?!<[^<>]*)(" +
												$.ui.autocomplete.escapeRegex(request.term) +
												")(?![^<>]*>)(?![^&;]+;)", "gi"
											), "<strong>$1</strong>"),
					          value: text,
					          option: this
					        };
					    }));
					  },
					  select: function (event, ui) {
					    ui.item.option.selected = true;
					    that._trigger("selected", event, {
					      item: ui.item.option
					    });
					  },
					  close: function (event, ui) {
					    that._trigger("close", event, ui);
					  },
					  change: function (event, ui) {
					    if (!ui.item) return removeIfInvalid(this);
					  }
					})
					.addClass("ui-widget ui-widget-content ui-corner-left");

      input.data("autocomplete")._renderItem = function (ul, item) {
        return $("<li>")
						.data("item.autocomplete", item)
						.append("<a>" + item.label + "</a>")
						.appendTo(ul);
      };

      $("<a>")
					.attr("tabIndex", -1)
					//.attr("title", "Show All Items")
					.tooltip()
					.appendTo(wrapper)
					.button({
					  icons: {
					    primary: "ui-icon-triangle-1-s"
					  },
					  text: false
					})
					.removeClass("ui-corner-all")
					.addClass("ui-corner-right ui-combobox-toggle")
					.click(function () {
					  // close if already visible
					  if (input.autocomplete("widget").is(":visible")) {
					    input.autocomplete("close");
					    removeIfInvalid(input);
					    return;
					  }

					  // work around a bug (likely same cause as #5265)
					  $(this).blur();

					  // pass empty string as value to search for, displaying all results
					  input.autocomplete("search", "");
					  input.focus();
					});

      input
						.tooltip({
						  position: {
						    of: this.button
						  },
						  tooltipClass: "ui-state-highlight"
						});
    },

		update: function () {
			var selected = this.element.children(":selected"),
			value = selected.val() ? selected.text() : "";
			this.input.val(value);
		},
		setValue: function (value) {
		  this.element.children(":selected").prop("selected", false);
			this.element.children('option[value="' + value + '"]').prop("selected", true);
			this.update();
		},

		search: function (value) {
			this.input.autocomplete('search', value).focus();
		},

    destroy: function () {
      this.wrapper.remove();
      this.element.show();
      $.Widget.prototype.destroy.call(this);
    }
  });
})(jQuery);
