'use strict';

/* Controllers */
var phonecatApp = angular.module('HomeApp', ['ngRoute', 'ngResource']);

/* Config */
phonecatApp.config([
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
          .when('/contact/:artId', {
              templateUrl: '/home/article',
              controller: 'HomeCtrl'
          })
          .otherwise({
              redirectTo: '/'
          });
  }
]);

/* Factory */

phonecatApp.factory('Article', [
  '$resource', function ($resource) {
      return $resource('/home/getarticle/:artId',
          {
              method: 'getTask',
              artId: 'articles'
          }, // Query parameters
          {
              'query': { method: 'GET' }

          });
      //Phone.update(params, successcb, errorcb);
  }
]);

/* Filter */
phonecatApp.filter('checkmark', function () {
    return function (input) {
        return input ? '\u2713' : '\u2718';
    }
});

phonecatApp.controller('PhoneListCtrl', [
  '$scope', '$http', '$location', 'Article',
  function ($scope, $http, $location, Article) {
      $scope.article = Article.query();

      console.log("Вывод - ", $scope.article.title);

      //Phone.query(params, successcb, errorcb)

      //Phone.get(params, successcb, errorcb)

      //Phone.save(params, payloadData, successcb, errorcb)

      //Phone.delete(params, successcb, errorcb)

  }
]);

/* Phone Detail Controller */
phonecatApp.controller('HomeCtrl', [
  '$scope', '$http', '$location', '$routeParams', 'Article',
  function ($scope, $http, $location, $routeParams, Article) {
      $scope.artId = $routeParams.artId;

      Article.get({ artId: $routeParams.artId }, function (data) {
          $scope.article = data;
          //data.$save();
      });

  }
]);
