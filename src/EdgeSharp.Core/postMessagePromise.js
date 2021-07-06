// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

window.external = {

    /* PostMessage Promise */
    EdgePromise: function Promise() {

        init();

        this.status = 'pending';
        this.requestId = getNextId();

        this.then = function (onSuccessCallback) {
            if (typeof onSuccessCallback === 'function') {
                EdgeExecuteResponses[onSuccessPrepend + this.requestId] = onSuccessCallback;
            }
            return this;
        };

        this.catch = function (onErrorCallback) {
            if (typeof onErrorCallback === 'function') {
                EdgeExecuteResponses[onErrorPrepend + this.requestId] = onErrorCallback;
            }
            return this;
        };

        this.send = function (request) {
            request.id = this.requestId;
            var requestJson = stringifyIfObject(request);
            window.chrome.webview.postMessage(requestJson);
            this.status = 'sent';
        };

        // Private methods
        function init() {
            window.onSuccessPrepend = "success_";
            window.onErrorPrepend = "error_";

            if (window.EdgeExecuteResponses === undefined ||
                window.EdgeExecuteResponses === null ||
                window.EdgeExecuteResponses.length <= 0) {
                window.EdgeExecuteResponses = {};
                window.EdgeExecuteRequestIds = [];
            }
        }

        function getNextId() {
            if (EdgeExecuteRequestIds === undefined ||
                EdgeExecuteRequestIds === null ||
                EdgeExecuteRequestIds.length <= 0) {
                var uniqueId = Math.random().toString(36).substring(2) + Date.now().toString(36);
                EdgeExecuteRequestIds.push(uniqueId);
                return uniqueId;
            }
            else {
                var uniqueId = EdgeExecuteRequestIds.pop();
                return uniqueId;
            }
        }

        function isRequestObject(request) {
            var type = typeof request;
            if (type === 'function' || type === 'object' && !!request) {
                return 1;
            }
            return 0;
        }

        function stringifyIfObject(objOrString) {
            var isObject = isRequestObject(objOrString);
            if (isObject === 1) {
                return JSON.stringify(objOrString);
            }
            return objOrString;
        }
    },

    Execute: function execute(url, content) {
        var request = {};
        request.url = url;
        if (content === undefined) {
            request.content = null;
        }
        else {
            request.content = content;
        }
        var handler = new window.external.EdgePromise();
        handler.send(request);
        return handler;
    },

    EdgeHandlerSuccessResponse: function handlerSuccessResponse(requestId, result) {
        var callback = EdgeExecuteResponses[onSuccessPrepend + requestId];
        if (callback) {
            callback(result);
            delete EdgeExecuteResponses[onSuccessPrepend + requestId];
            EdgeExecuteRequestIds.push(requestId);
        }

        return "success";
    },

    EdgeHandlerErrorResponse: function handlerErrorResponse(requestId, error) {
        var callback = EdgeExecuteResponses[onErrorPrepend + requestId];
        if (callback) {
            callback(error);
            delete EdgeExecuteResponses[onErrorPrepend + requestId];
            EdgeExecuteRequestIds.push(requestId);
        }

        return "error";
    }
};