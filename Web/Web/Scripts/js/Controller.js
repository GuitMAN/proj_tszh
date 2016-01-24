;
'use strict';

/* Controllers */
var HomeApp = angular.module('HomeApp', ['ngRoute', 'ngResource']);

/* Config */
HomeApp.config([
  '$routeProvider', '$locationProvider',
  function ($routeProvide, $locationProvider) {
      $routeProvide
          .when('/', {
              templateUrl: '/home/article',
              controller: 'HomeCtrl'
          })
          .when('/about/:artId', {
              templateUrl: '/home/article',
              controller: 'HomeCtrl'
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

HomeApp.controller('PhoneListCtrl', [
  '$scope', '$http', '$location', 'Article',
  function ($scope, $http, $location, Article) {
      $scope.article = Article.query();

      console.log("Вывод - ", $scope.article.title);

  }
]);

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
