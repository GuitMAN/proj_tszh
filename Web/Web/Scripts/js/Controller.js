;
'use strict';

/* Controllers */
var HomeApp = angular.module('HomeApp', ['ngRoute', 'ngResource']);

/* Config */
HomeApp.config([
  '$routeProvider', '$locationProvider',
  function ($routeProvide, $locationProvider) {
      //$locationProvider.html5Mode({
      //    enabled: true,
      //    requestMode: false
      //});
      $routeProvide
          .when('/', {
              templateUrl: '/home/article',
              controller: 'HomeCtrl'
          })
          .when('/article/:artId', {
              templateUrl: '/home/article',
              controller: 'HomeCtrl'
          })
          .when('/test', {
              templateUrl: '/login/index',
              controller: 'TestCtrl'
          })
          .otherwise({
              redirectTo: '/'
          });
  }
]);

/* Factory */

HomeApp.factory('Article', [
  '$resource', function ($resource) {
      return $resource('/home/getarticle/:artId',
          {
              method: 'getTask',
              artId: 'articles'
          }, 
          {
              'query': { method: 'GET' }

          });
      
  }
]);

/* Filter */
HomeApp.filter('checkmark', function () {
    return function (input) {
        return input ? '\u2713' : '\u2718';
    }
});

HomeApp.filter('aspDate', function () {
    'use strict';
    return function (input) {
        if (input) {
            return parseInt(input.substr(6));
        }
        else {
            return;
        }
    };
});




/* Phone Detail Controller */
HomeApp.controller('HomeCtrl', [
  '$scope', '$http', '$location', '$routeParams', 'Article',
  function ($scope, $http, $location, $routeParams, Article) {
      $scope.artId = $routeParams.artId;

      Article.get({ artId: $routeParams.artId }, function (data) {
          $scope.article = data;

      });

  }
]);




HomeApp.factory('Test', [
  '$resource', function ($resource) {
      return $resource('/home/test',
          {
              method: 'getTask',
              author: 'Пиздец вещает'
          },
          {
              'save': { method: 'POST' }
          });

  }
]);


HomeApp.controller('TestCtrl', [
  '$scope', '$http', '$location', '$routeParams', 'Test',
  function ($scope, $http, $location, $routeParams, Test) {
      $scope.login = $routeParams.author;
      $scope.autoApplications = "Y";
      $scope.save = function (login, answerForm)
      {
          if (answerForm.$valid) {
              
              $http.post("/Login?AspxAutoDetectCookieSupport=1", $scope.login).then(function (data) {
                  $scope.status = data.status;
                  $scope.data = data.statusText;
                  if (data.status==200)
                        $scope.autoApplications = "N";

              },function (data) {
                  $scope.status = data.status;
                  $scope.data = data.statusText;
              }) ;
          }
      }




  }
]);