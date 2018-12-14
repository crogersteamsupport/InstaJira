/**
 * Compiled inline version. (Library mode)
 */

/*jshint smarttabs:true, undef:true, latedef:true, curly:true, bitwise:true, camelcase:true */
/*globals $code */

(function(exports, undefined) {
	"use strict";

	var modules = {};

	function require(ids, callback) {
		var module, defs = [];

		for (var i = 0; i < ids.length; ++i) {
			module = modules[ids[i]] || resolve(ids[i]);
			if (!module) {
				throw 'module definition dependecy not found: ' + ids[i];
			}

			defs.push(module);
		}

		callback.apply(null, defs);
	}

	function define(id, dependencies, definition) {
		if (typeof id !== 'string') {
			throw 'invalid module definition, module id must be defined and be a string';
		}

		if (dependencies === undefined) {
			throw 'invalid module definition, dependencies must be specified';
		}

		if (definition === undefined) {
			throw 'invalid module definition, definition function must be specified';
		}

		require(dependencies, function() {
			modules[id] = definition.apply(null, arguments);
		});
	}

	function defined(id) {
		return !!modules[id];
	}

	function resolve(id) {
		var target = exports;
		var fragments = id.split(/[.\/]/);

		for (var fi = 0; fi < fragments.length; ++fi) {
			if (!target[fragments[fi]]) {
				return;
			}

			target = target[fragments[fi]];
		}

		return target;
	}

	function expose(ids) {
		var i, target, id, fragments, privateModules;

		for (i = 0; i < ids.length; i++) {
			target = exports;
			id = ids[i];
			fragments = id.split(/[.\/]/);

			for (var fi = 0; fi < fragments.length - 1; ++fi) {
				if (target[fragments[fi]] === undefined) {
					target[fragments[fi]] = {};
				}

				target = target[fragments[fi]];
			}

			target[fragments[fragments.length - 1]] = modules[id];
		}
		
		// Expose private modules for unit tests
		if (exports.AMDLC_TESTS) {
			privateModules = exports.privateModules || {};

			for (id in modules) {
				privateModules[id] = modules[id];
			}

			for (i = 0; i < ids.length; i++) {
				delete privateModules[ids[i]];
			}

			exports.privateModules = privateModules;
		}
	}

// Included from: js/DomTextMatcher.js

/**
 * DomTextMatcher.js
 *
 * Released under LGPL License.
 * Copyright (c) 1999-2015 Ephox Corp. All rights reserved
 *
 * License: http://www.tinymce.com/license
 * Contributing: http://www.tinymce.com/contributing
 */

/*eslint no-labels:0, no-constant-condition: 0 */

/**
 * This class logic for filtering text and matching words.
 *
 * @class tinymce.tinymcespellcheckerplugin.TextFilter
 * @private
 */
define("tinymce/tinymcespellcheckerplugin/DomTextMatcher", [], function() {
	// Based on work developed by: James Padolsey http://james.padolsey.com
	// released under UNLICENSE that is compatible with LGPL
	// TODO: Handle contentEditable edgecase:
	// <p>text<span contentEditable="false">text<span contentEditable="true">text</span>text</span>text</p>
	return function(node, editor, wordCharPattern) {
		var m, dom = editor.dom;
		var blockElementsMap, hiddenTextElementsMap, shortEndedElementsMap, ignoreClass = "mce-spellchecker-ignore";

		blockElementsMap = editor.schema.getBlockElements(); // H1-H6, P, TD etc
		hiddenTextElementsMap = editor.schema.getWhiteSpaceElements(); // TEXTAREA, PRE, STYLE, SCRIPT
		shortEndedElementsMap = editor.schema.getShortEndedElements(); // BR, IMG, INPUT

		function createMatch(m, data) {
			if (!m[0]) {
				throw 'findAndReplaceDOMText cannot handle zero-length matches';
			}

			return {
				start: m.index,
				end: m.index + m[0].length,
				text: m[0],
				data: data
			};
		}

		function getText(node) {
			var txt;

			if (node.nodeType === 3) {
				return node.data;
			}

			if (hiddenTextElementsMap[node.nodeName] && !blockElementsMap[node.nodeName]) {
				return '';
			}

			txt = '';

			if (blockElementsMap[node.nodeName] || shortEndedElementsMap[node.nodeName]) {
				txt += '\n';
			}

			if ((node = node.firstChild)) {
				do {
					txt += getText(node);
				} while ((node = node.nextSibling));
			}

			return txt;
		}

		function getUniqueValuesFromArray(arrayToCheck) {
			function arrayContains(arrayToCheck, value) {
				for (var i = 0; i < arrayToCheck.length; i++) {
					if (arrayToCheck[i] === value) {
						return true;
					}
				}

				return false;
			}

			var arrayToKeep = [];

			for (var i = 0; i < arrayToCheck.length; i++) {
				if (!arrayContains(arrayToKeep, arrayToCheck[i])) {
					arrayToKeep.push(arrayToCheck[i]);
				}
			}

			return arrayToKeep;
		}

		function getWords(node) {
			var matches = getText(node).match(wordCharPattern);
			return matches ? getUniqueValuesFromArray(matches) : [];
		}

		function stepThroughMatches(node, matches, replaceFn) {

			var startNode, endNode, startNodeIndex,
				endNodeIndex, innerNodes = [], atIndex = 0, curNode = node,
				matchLocation, matchIndex = 0;

			matches = matches.slice(0);
			matches.sort(function(a, b) {
				return a.start - b.start;
			});

			matchLocation = matches.shift();

			out: while (true) {
				if (blockElementsMap[curNode.nodeName] || shortEndedElementsMap[curNode.nodeName]) {
					atIndex++;
				}

				if (curNode.nodeType === 3) {
					if (!endNode && curNode.length + atIndex >= matchLocation.end) {
						// We've found the ending
						endNode = curNode;
						endNodeIndex = matchLocation.end - atIndex;
					} else if (startNode) {
						// Intersecting node
						innerNodes.push(curNode);
					}

					if (!startNode && curNode.length + atIndex > matchLocation.start) {
						// We've found the match start
						startNode = curNode;
						startNodeIndex = matchLocation.start - atIndex;
					}

					atIndex += curNode.length;
				}

				if (startNode && endNode) {
					//We need to check if this is an ignored element
					var ignored = false;
					for (var currNode = startNode; currNode != node; currNode = currNode.parentNode) {
						if (currNode.className === ignoreClass) {
							ignored = true;
						}
					}

					if (!ignored) {
						curNode = replaceFn({
							startNode: startNode,
							startNodeIndex: startNodeIndex,
							endNode: endNode,
							endNodeIndex: endNodeIndex,
							innerNodes: innerNodes,
							match: matchLocation.text,
							matchIndex: matchIndex
						});

						// replaceFn has to return the node that replaced the endNode
						// and then we step back so we can continue from the end of the
						// match:
						atIndex -= (endNode.length - endNodeIndex);
					}

					startNode = null;
					endNode = null;
					innerNodes = [];
					matchLocation = matches.shift();
					matchIndex++;

					if (!matchLocation) {
						break; // no more matches
					}
				} else if ((!hiddenTextElementsMap[curNode.nodeName] || blockElementsMap[curNode.nodeName]) && curNode.firstChild) {
					// Move down
					curNode = curNode.firstChild;
					continue;
				} else if (curNode.nextSibling) {
					// Move forward:
					curNode = curNode.nextSibling;
					continue;
				}

				// Move forward or up:
				while (true) {
					if (curNode.nextSibling) {
						curNode = curNode.nextSibling;
						break;
					} else if (curNode.parentNode !== node) {
						curNode = curNode.parentNode;
					} else {
						break out;
					}
				}
			}
		}

		/**
		* Generates the actual replaceFn which splits up text nodes
		* and inserts the replacement element.
		*/
		function genReplacer(matches, callback) {
			function makeReplacementNode(fill, matchIndex) {
				var match = matches[matchIndex];

				if (!match.stencil) {
					match.stencil = callback(match);
				}

				var clone = match.stencil.cloneNode(false);
				clone.setAttribute('data-mce-index', matchIndex);

				if (fill) {
					clone.appendChild(dom.doc.createTextNode(fill));
				}

				return clone;
			}

			return function(range) {
				var before, after, parentNode, startNode = range.startNode,
					endNode = range.endNode, matchIndex = range.matchIndex,
					doc = dom.doc;

				if (startNode === endNode) {
					var node = startNode;

					parentNode = node.parentNode;
					if (range.startNodeIndex > 0) {
						// Add "before" text node (before the match)
						before = doc.createTextNode(node.data.substring(0, range.startNodeIndex));
						parentNode.insertBefore(before, node);
					}

					// Create the replacement node:
					var el = makeReplacementNode(range.match, matchIndex);
					parentNode.insertBefore(el, node);
					if (range.endNodeIndex < node.length) {
						// Add "after" text node (after the match)
						after = doc.createTextNode(node.data.substring(range.endNodeIndex));
						parentNode.insertBefore(after, node);
					}

					node.parentNode.removeChild(node);

					return el;
				}

				// Replace startNode -> [innerNodes...] -> endNode (in that order)
				before = doc.createTextNode(startNode.data.substring(0, range.startNodeIndex));
				after = doc.createTextNode(endNode.data.substring(range.endNodeIndex));
				var elA = makeReplacementNode(startNode.data.substring(range.startNodeIndex), matchIndex);
				var innerEls = [];

				for (var i = 0, l = range.innerNodes.length; i < l; ++i) {
					var innerNode = range.innerNodes[i];
					var innerEl = makeReplacementNode(innerNode.data, matchIndex);
					innerNode.parentNode.replaceChild(innerEl, innerNode);
					innerEls.push(innerEl);
				}

				var elB = makeReplacementNode(endNode.data.substring(0, range.endNodeIndex), matchIndex);

				parentNode = startNode.parentNode;
				parentNode.insertBefore(before, startNode);
				parentNode.insertBefore(elA, startNode);
				parentNode.removeChild(startNode);

				parentNode = endNode.parentNode;
				parentNode.insertBefore(elB, endNode);
				parentNode.insertBefore(after, endNode);
				parentNode.removeChild(endNode);

				return elB;
			};
		}

		/**
		 * Returns the index of a specific match object or -1 if it isn't found.
		 *
		 * @param  {Match} match Text match object.
		 * @return {Number} Index of match or -1 if it isn't found.
		 */
		function indexOf(matches, match) {
			var i = matches.length;
			while (i--) {
				if (matches[i] === match) {
					return i;
				}
			}

			return -1;
		}

		/**
		 * Filters the matches. If the callback returns true it stays if not it gets removed.
		 *
		 * @param {Function} callback Callback to execute for each match.
		 * @return {DomTextMatcher} Current DomTextMatcher instance.
		 */
		function filter(matches, callback) {
			var filteredMatches = [];

			each(matches, function(match, i) {
				if (callback(match, i)) {
					filteredMatches.push(match);
				}
			});

			matches = filteredMatches;

			/*jshint validthis:true*/
			return filteredMatches;
		}

		/**
		 * Executes the specified callback for each match.
		 *
		 * @param {Function} callback  Callback to execute for each match.
		 * @return {DomTextMatcher} Current DomTextMatcher instance.
		 */
		function each(matches, callback) {
			for (var i = 0, l = matches.length; i < l; i++) {
				if (callback(matches[i], i) === false) {
					break;
				}
			}

			/*jshint validthis:true*/
			return this;
		}

		/**
		 * Wraps the current matches with nodes created by the specified callback.
		 * Multiple clones of these matches might occur on matches that are on multiple nodex.
		 *
		 * @param {Function} callback Callback to execute in order to create elements for matches.
		 * @return {DomTextMatcher} Current DomTextMatcher instance.
		 */
		function wrap(matches, callback, node) {
			if (matches.length) {
				stepThroughMatches(node, matches, genReplacer(matches, callback));
			}

			/*jshint validthis:true*/
			return this;
		}

		/**
		 * Finds the specified regexp and adds them to the matches collection.
		 *
		 * @param {RegExp} regex Global regexp to search the current node by.
		 * @param {Object} [data] Optional custom data element for the match.
		 * @return {DomTextMatcher} Current DomTextMatcher instance.
		 */
		function find(regex, data, node) {
			var matches = [];
			var textToMatch = getText(node);
			if (textToMatch && regex.global) {
				while ((m = regex.exec(textToMatch))) {
					matches.push(createMatch(m, data));
				}
			}

			return matches;
		}

		return {
			getText: getText,
			getWords: getWords,
			each: each,
			filter: filter,
			find: find,
			wrap: wrap,
			indexOf: indexOf
		};
	};
});

// Included from: js/SpellingCache.js

/**
 * SpellingCache.js
 *
 * Released under LGPL License.
 * Copyright (c) 1999-2015 Ephox Corp. All rights reserved
 *
 * License: http://www.tinymce.com/license
 * Contributing: http://www.tinymce.com/contributing
 */

/**
 * This class is used to store spelling suggestions and more in one place.
 *
 * @class tinymce.tinymcespellcheckerplugin.TextFilter
 * @private
 */
define("tinymce/tinymcespellcheckerplugin/SpellingCache", [
	"tinymce/util/JSON"
], function(JSON) {
	return function() {
		var storageItemName = "tinyMCESpellingCache",
		fallbackCache = {},
		suggestionsLimit = 5;

		function cleanWord(word) {
			return word.replace(/\uFEFF/g, '');
		}

		function addWordToCache(language, word, misspelled, suggestions) {
			var spellingCache = getCache(language),
				fixedWord = cleanWord(word),
				cacheObject = {
					misspelled: misspelled,
					suggestions: suggestions
				};
			spellingCache[language][fixedWord] = cacheObject;

			if (storageSupported()) {
				sessionStorage.setItem(storageItemName, JSON.serialize(spellingCache));
			} else {
				fallbackCache = spellingCache;
			}
		}

		function storageSupported() {
			return typeof (Storage) !== "undefined";
		}

		function checkCacheLanguage(language, spellingCache) {
			if (!spellingCache[language]) {
				spellingCache[language] = {};
			}
			return spellingCache;
		}

		function getCache(language) {
			//if the cache isn't there yet
			var spellingCache = storageSupported() && sessionStorage.getItem(storageItemName) ?
				JSON.parse(sessionStorage.getItem(storageItemName)) : fallbackCache;
			return checkCacheLanguage(language, spellingCache);
		}

		function retrieveWordFromCache(language, word) {
			var spellingCache = getCache(language),
				fixedWord = cleanWord(word),
				cacheObject = spellingCache[language][fixedWord] ? spellingCache[language][fixedWord] : false;

			//If the suggestions array exists, and has a length greater than
			if (cacheObject && cacheObject.suggestions && cacheObject.suggestions.length > suggestionsLimit) {
				//limit the suggestions
				cacheObject.suggestions = cacheObject.suggestions.slice(0, suggestionsLimit);
			}

			return cacheObject;
		}

		return {
			addWordToCache: addWordToCache,
			retrieveWordFromCache: retrieveWordFromCache
		};
	};
});

// Included from: js/Plugin.js

/**
 * Plugin.js
 *
 * Released under LGPL License.
 * Copyright (c) 1999-2015 Ephox Corp. All rights reserved
 *
 * License: http://www.tinymce.com/license
 * Contributing: http://www.tinymce.com/contributing
 */

/*jshint camelcase:false */

/**
 * This class contains all core logic for the spellchecker plugin.
 *
 * @class tinymce.tinymcespellcheckerplugin.Plugin
 * @private
 */
define("tinymce/tinymcespellcheckerplugin/Plugin", [
	"tinymce/tinymcespellcheckerplugin/DomTextMatcher",
	"tinymce/tinymcespellcheckerplugin/SpellingCache",
	"tinymce/PluginManager",
	"tinymce/util/Tools",
	"tinymce/ui/Menu",
	"tinymce/dom/DOMUtils",
	"tinymce/util/XHR",
	"tinymce/util/URI",
	"tinymce/util/JSON"
], function(DomTextMatcher, SpellingCache, PluginManager, Tools, Menu, DOMUtils, XHR, URI, JSON) {
	PluginManager.add('tinymcespellchecker', function(editor, url) {
		var self = this, suggestionsMenu, settings = editor.settings, spellcheckerVersion = 1,
		word_regexp = '[' + '\\w' + '\'' + '\\-' + '\\u00C0-\\u00FF' + '\\uFEFF' + '\\u2018\\u2019' + ']+',
		languagesString = settings.spellchecker_languages ||
			'US English=en,UK English=en_gb,Danish=da,Dutch=nl,Finnish=fi,French=fr,' +
			'German=de,Italian=it,Norwegian=nb,Brazilian Portuguese=pt,Iberian Portuguese=pt_pt,' +
			'Spanish=es,Swedish=sv',
		languageMenuItems, wordClass = "mce-spellchecker-word", ignoreClass = "mce-spellchecker-ignore", currentParentNode,
		spellchecking = true, spellcheckButton, spellcheckMenuItem;

		function getTextMatcher() {
			if (!self.textMatcher) {
				self.textMatcher = new DomTextMatcher(editor.getBody(), editor, getWordCharPattern());
			}

			return self.textMatcher;
		}

		function getSpellingCache() {
			if (!self.spellingCache) {
				self.spellingCache = new SpellingCache();
			}

			return self.spellingCache;
		}

		function showSuggestions(word, spans) {
			var items = [], suggestions = getSpellingCache().retrieveWordFromCache(settings.spellchecker_language, word).suggestions;

			Tools.each(suggestions, function(suggestion) {
				items.push({
					text: suggestion,
					onclick: function() {
						editor.insertContent(editor.dom.encode(suggestion));
						editor.dom.remove(spans);
					}
				});
			});

			items.push({text: '-'});

			items.push.apply(items, [
				{text: 'Ignore', onclick: function() {
					ignoreWord(word, spans);
				}},

				{text: 'Ignore all', onclick: function() {
					ignoreWord(word, spans, true);
				}}
			]);

			// Render menu
			suggestionsMenu = new Menu({
				items: items,
				context: 'contextmenu',
				onautohide: function(e) {
					if (e.target.className.indexOf('spellchecker') != -1) {
						e.preventDefault();
					}
				},
				onhide: function() {
					suggestionsMenu.remove();
					suggestionsMenu = null;
				}
			});

			suggestionsMenu.renderTo(document.body);

			// Position menu
			var pos = DOMUtils.DOM.getPos(editor.getContentAreaContainer());
			var targetPos = editor.dom.getPos(spans[0]);
			var root = editor.dom.getRoot();

			// Adjust targetPos for scrolling in the editor
			if (root.nodeName == 'BODY') {
				targetPos.x -= root.ownerDocument.documentElement.scrollLeft || root.scrollLeft;
				targetPos.y -= root.ownerDocument.documentElement.scrollTop || root.scrollTop;
			} else {
				targetPos.x -= root.scrollLeft;
				targetPos.y -= root.scrollTop;
			}

			pos.x += targetPos.x;
			pos.y += targetPos.y;

			suggestionsMenu.moveTo(pos.x, pos.y + spans[0].offsetHeight);
		}

		function getWordCharPattern() {
			// Regexp for finding word specific characters this will split words by
			// spaces, quotes, copy right characters etc. It's escaped with unicode characters
			// to make it easier to output scripts on servers using different encodings
			// so if you add any characters outside the 128 byte range make sure to escape it
			return editor.getParam('spellchecker_wordchar_pattern') || new RegExp(word_regexp, "g");
		}

		function cleanWord(word) {
			return word.replace(/\uFEFF/g, '');
		}

		function getCachedCleanedWords(words) {
			var cachedData = {},
				wordsToCheck = [],
				cleanedWordsForServer = [];

			//For each word we were going to send to the server
			for (var i = 0; i < words.length; i++) {
				//Attempt to get the cached word instead
				var cachedWord = getSpellingCache().retrieveWordFromCache(settings.spellchecker_language, words[i]);

				//If we have the word cached already and it's misspelled
				//we need to add it to the cachedData object under the unclean version of the word
				if (cachedWord && cachedWord.misspelled) {
					cachedData[words[i]] = cachedWord;
				}

				if (!cachedWord) { //Otherwise if we don't have it cached yet
					wordsToCheck.push({
						original: words[i],
						cleaned: cleanWord(words[i])
					});
					cleanedWordsForServer.push(cleanWord(words[i]));
				}
			}

			return {
				cachedData: cachedData,
				wordsToCheck: wordsToCheck,
				cleanedWordsForServer: cleanedWordsForServer
			};
		}

		function makeServerRequest(data, cachedCleanedWords, node) {
			var postSuffix = "/" + spellcheckerVersion + "/correction";

			function errorCallback(message) {
				editor.windowManager.alert(message);
			}

			var request = {
				url: new URI(url).toAbsolute(settings.spellchecker_rpc_url + postSuffix),
				type: "post",
				content_type: 'application/json',
				data: JSON.serialize(data),
				success: function(result) {
					result = JSON.parse(result);

					if (!result) {
						errorCallback("Server response wasn't proper JSON.");
					} else if (result.error) {
						errorCallback(result.error);
					} else {
						cacheAndMarkSuggestions(cachedCleanedWords.cachedData, result, cachedCleanedWords.wordsToCheck, node);
					}
				},
				error: function(type, xhr) {
					errorCallback("Spellchecker request error: " + xhr.status);
				}
			};

			if (settings.spellchecker_api_key) {
				request.requestheaders = [
					{
						key: 'tiny-api-key',
						value: settings.spellchecker_api_key
					}
				];
			}

			return request;
		}

		function spellcheckServerCall(words, node) {
			var cachedCleanedWords = getCachedCleanedWords(words);

			var data = {
				words: cachedCleanedWords.cleanedWordsForServer,
				language: settings.spellchecker_language
			};

			//If we have some words to check
			if (cachedCleanedWords.wordsToCheck.length > 0) {
				var request = makeServerRequest(data, cachedCleanedWords, node);
				XHR.send(request);
			} else {
				cacheAndMarkSuggestions(cachedCleanedWords.cachedData, {}, cachedCleanedWords.wordsToCheck, node);
			}
		}

		function spellcheck(node) {
			spellcheckServerCall(getTextMatcher().getWords(node), node);
			editor.focus();
		}

		function ignoreWord(word, spans, all) {
			editor.selection.collapse();

			if (all) {
				Tools.each(editor.dom.select('span.' + wordClass), function(span) {
					if (span.getAttribute('data-mce-word') == word) {
						//editor.dom.remove(span, true);
						span.className = ignoreClass;
					}
				});
			} else {
				//editor.dom.remove(spans, true);
				Tools.each(spans, function(span) {
					span.className = ignoreClass;
				});
			}

		}

		function getElmIndex(elm) {
			var value = elm.getAttribute('data-mce-index');

			if (typeof value == "number") {
				return "" + value;
			}

			return value;
		}

		function findSpansByIndex(index, node) {
			var nodes, spans = [];

			nodes = Tools.toArray(node.getElementsByTagName('span'));
			if (nodes.length) {
				for (var i = 0; i < nodes.length; i++) {
					var nodeIndex = getElmIndex(nodes[i]);

					if (nodeIndex === null || !nodeIndex.length) {
						continue;
					}

					if (nodeIndex === index.toString()) {
						spans.push(nodes[i]);
					}
				}
			}

			return spans;
		}

		function updateSelection(e) {
			var selectedLanguage = settings.spellchecker_language;

			Tools.each(e.control.items(), function(item) {
				item.active(item.data.data === selectedLanguage);
			});
		}

		function setUpActiveStateToggle(e) {
			e.control.on('show', function(ee) {
				updateSelection(ee);
			});
		}

		function setUpActiveSpellcheckingToggle(e) {
			spellcheckMenuItem = e.control;
			e.control.on('show', function() {
				this.active(spellchecking);
			});
			spellcheckMenuItem.active(spellchecking);
		}

		function cacheAndCombineResults(cachedData, result, words) {
			var combinedResults = {};

			//Add cached results to combined data object
			for (var cachedField in cachedData) {
				if (cachedData[cachedField] && cachedData[cachedField].misspelled) {
					combinedResults[cachedField] = cachedData[cachedField].suggestions;
				}
			}

			//For each misspelled result we have from the server, we need to add it to the cache.
			for (var field in result.spell) {
				getSpellingCache().addWordToCache(settings.spellchecker_language, field, true, result.spell[field]);
				//For each word we were going to check, if the cleaned version of the word matches the currently being checked field
				for (var w = 0; w < words.length; w++) {
					//We've found our word
					if (words[w].cleaned === field) {
						combinedResults[words[w].original] = result.spell[field];
						break;
					}
				}
			}

			return combinedResults;
		}

		function cacheCorrectlySpelledWords(words) {
			//Finally, now that we have the combinedresults of a cache and service
			for (var i = 0; i < words.length; i++) {
				//If a word we were searching for isn't cached, and isn't part of the results
				//We need to assume that the word is in the dictionary
				var cachedWord = getSpellingCache().retrieveWordFromCache(settings.spellchecker_language, words[i].cleaned);

				if (!cachedWord) {
					getSpellingCache().addWordToCache(settings.spellchecker_language, words[i].cleaned, false, []);
				}
			}
		}

		function cacheAndMarkSuggestions(cachedData, result, words, node) {
			var combinedResults = cacheAndCombineResults(cachedData, result, words);

			cacheCorrectlySpelledWords(words);

			markErrors(combinedResults, node);
		}

		function unwrapWordNode(wordNode) {
			var parentNode = wordNode.parentNode;
			var childNodes = wordNode.childNodes;
			while (childNodes.length > 0) {
				parentNode.insertBefore(childNodes[0], wordNode);
			}
			wordNode.parentNode.removeChild(wordNode);
		}

		function clearWordMarks(node) {
			var wrappedWords = editor.dom.select('span.' + wordClass, node);

			//Unwrap from node
			for (var i = 0; i < wrappedWords.length; i++) {
				unwrapWordNode(wrappedWords[i]);
			}
		}

		/**
		 * Find the specified words and marks them. It will also show suggestions for those words.
		 *
		 * @example
		 * editor.plugins.tinymcespellcheckerplugin.markErrors({
		 *     dictionary: true,
		 *     words: {
		 *         "word1": ["suggestion 1", "Suggestion 2"]
		 *     }
		 * });
		 * @param {Object} data Data object containing the words with suggestions.
		 */
		function markErrors(data, node) {

			var bm = editor.selection.getBookmark();

			clearWordMarks(node);

			var matches = getTextMatcher().find(getWordCharPattern(), undefined, node);
			matches = getTextMatcher().filter(matches, function(match) {
				return !!data[match.text];
			});

			getTextMatcher().wrap(matches, function(match) {
				return editor.dom.create('span', {
					"class": wordClass,
					"data-mce-bogus": 1,
					"data-mce-word": match.text
				});
			}, node);

			editor.selection.moveToBookmark(bm);
		}

		function getParentBlock(currNode) {
			var rootNode = editor.getBody(),
			parentNode = rootNode, blockElements = editor.schema.getBlockElements();

			for (currNode = currNode; currNode != rootNode; currNode = currNode.parentNode) {
				if (currNode === null || currNode === undefined) {
					break;
				}
				if (blockElements[currNode.nodeName]) {
					parentNode = currNode;
					break;
				}
			}

			return parentNode;
		}

		function buildMenuItems(listName, languageValues) {
			var items = [];

			Tools.each(languageValues, function(languageValue) {
				items.push({
					selectable: true,
					text: languageValue.name,
					data: languageValue.value,
					onclick: function() {
						settings.spellchecker_language = languageValue.value;
						if (spellchecking) {
							spellcheck(editor.getBody());
						}
					}
				});
			});

			return items;
		}

		languageMenuItems = buildMenuItems('Language',
			Tools.map(languagesString.split(','), function(langPair) {
				langPair = langPair.split('=');

				return {
					name: langPair[0],
					value: langPair[1]
				};
			})
		);

		function toggleSpellcheck() {
			if (spellchecking) {
				unBindEvents();
				clearWordMarks(editor.getBody());
			} else {
				bindEvents();
				spellcheck(editor.getBody());
			}
			spellchecking = !spellchecking;
			spellcheckButton.active(spellchecking);
			if (spellcheckMenuItem) {
				spellcheckMenuItem.active(spellchecking);
			}
		}

		var buttonArgs = {
			tooltip: 'Spellcheck',
			onclick: toggleSpellcheck,
			onPostRender: function(e) {
				e.control.active(spellchecking);
			}
		};

		if (languageMenuItems.length > 1) {
			buttonArgs.type = 'splitbutton';
			buttonArgs.menu = languageMenuItems;
			buttonArgs.onshow = updateSelection;
			buttonArgs.onpostrender = function(e) {
				spellcheckButton = e.control;
			};
		}

		editor.addButton('spellchecker', buttonArgs);
		editor.addCommand('mceSpellCheck', toggleSpellcheck);

		editor.addMenuItem('spellchecker', {
			text: 'Spellcheck',
			context: 'tools',
			onclick: toggleSpellcheck,
			onPostRender: setUpActiveSpellcheckingToggle,
			selectable: true
		});

		editor.addMenuItem('spellcheckerlanguage', {
			text: 'Spellcheck Language',
			context: 'tools',
			menu: languageMenuItems,
			onPostRender: setUpActiveStateToggle
		});

		function editorRemove() {
			if (suggestionsMenu) {
				suggestionsMenu.remove();
				suggestionsMenu = null;
			}
		}

		function editorKeydown(e) {
			//We probably don't want to unwrap the word we're in if we're just pressing arrow keys around
			if ((e.which || e.keycode) === 37 ||
				(e.which || e.keycode) === 38 ||
				(e.which || e.keycode) === 39 ||
				(e.which || e.keycode) === 40) {
				return;
			}

			var currNode = editor.selection.getNode();

			if (DOMUtils.DOM.hasClass(currNode, wordClass) || DOMUtils.DOM.hasClass(currNode, ignoreClass)) {

				var parentNode = getParentBlock(currNode),
					wordNodes = findSpansByIndex(getElmIndex(currNode), parentNode);

				//Unwrap the ignored/misspelled word
				if (wordNodes.length > 0) {
					var bm = editor.selection.getBookmark();
					for (var i = 0; i < wordNodes.length; i++) {
						unwrapWordNode(wordNodes[i]);
					}
					editor.selection.moveToBookmark(bm);
				}
			}
		}

		function editorKeyup(e) {

			var currNode = editor.selection.getNode(),
				parentNode = getParentBlock(currNode);

			if ((e.which || e.keycode) === 32) {

				spellcheck(parentNode);

			}
		}

		function editorNodeChange(e) {

			//We don't really need two events...
			if (e.selectionChange) {
				return;
			}

			var previousParentNode = currentParentNode;

			currentParentNode = getParentBlock(e.element);

			//If the node we just changed from is the exact same as the current node
			//we should prevent sending a request
			if (previousParentNode === currentParentNode) {
				return;
			}

			if (previousParentNode) {
				spellcheck(previousParentNode);
			}

		}

		function editorContextMenu(e) {
			var target = e.target;

			if (DOMUtils.DOM.hasClass(target, wordClass)) {

				var spans = findSpansByIndex(getElmIndex(target), getParentBlock(target));

				if (spans.length > 0) {
					var rng = editor.dom.createRng();
					rng.setStartBefore(spans[0]);
					rng.setEndAfter(spans[spans.length - 1]);
					editor.selection.setRng(rng);
					showSuggestions(target.getAttribute('data-mce-word'), spans, e);
				}

				e.preventDefault();
				e.stopImmediatePropagation();
			}
		}

		function bindEvents() {
			editor.on('remove', editorRemove);
			editor.on('keydown', editorKeydown);
			editor.on('keyup', editorKeyup);
			editor.on('nodechange', editorNodeChange);
		}

		function unBindEvents() {
			editor.off('remove', editorRemove);
			editor.off('keydown', editorKeydown);
			editor.off('keyup', editorKeyup);
			editor.off('nodechange', editorNodeChange);
		}

		editor.on('contextmenu', editorContextMenu, true);

		bindEvents();

		this.getTextMatcher = getTextMatcher;
		this.getWordCharPattern = getWordCharPattern;
		this.markErrors = markErrors;
		this.getLanguage = function() {
			return settings.spellchecker_language;
		};

		// Set default spellchecker language if it's not specified
		settings.spellchecker_language = settings.spellchecker_language || settings.language || 'en';
	});
});

expose(["tinymce/tinymcespellcheckerplugin/DomTextMatcher","tinymce/tinymcespellcheckerplugin/SpellingCache"]);
})(this);