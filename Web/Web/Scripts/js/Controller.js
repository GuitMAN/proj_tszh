;
'use strict';

/* Controllers */



var HomeApp = angular.module('HomeApp', ['ui.bootstrap', 'ngAnimate', 'ngRoute', 'ngResource', 'ngCookies']);

ArrMonth = [
  { id: '1', name: 'Январь' },
  { id: '2', name: 'Февраль' },
  { id: '3', name: 'Март' },
  { id: '4', name: 'Апрель' },
  { id: '5', name: 'Май' },
  { id: '6', name: 'Июнь' },
  { id: '7', name: 'Июль' },
  { id: '8', name: 'Август' },
  { id: '8', name: 'Сентябрь' },
  { id: '10', name: 'Октябрь' },
  { id: '11', name: 'Ноябрь' },
  { id: '12', name: 'Декабрь' }
];

HomeApp.filter('JsonDate', function () {
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

/* Config */
HomeApp.config([
  '$routeProvider', '$locationProvider',
  function ($routeProvide, $locationProvider) {
     // $locationProvider.html5Mode(true);
      $routeProvide
          .when('/', {
              templateUrl: '/home/article',
              controller: 'HomeCtrl'
          })
          .when('/article/:artId', {
              templateUrl: '/home/article',
              controller: 'HomeCtrl'
          })
          .when('/login', {
              templateUrl: '/login/index',
              controller: 'LoginCtrl'
          })
          .when('/logoout', {
              templateUrl: '/login/logoout',
              controller: 'LogoutCtrl'
          })
          .when('/register', {
              templateUrl: '/login/register',
              controller: 'RegisterCtrl'
          })
          .when('/manage', {
              templateUrl: '/login/manage',
              controller: 'FeedbackCtrl'
          })
          .when('/feedback', {
              templateUrl: '/user/feedback',
              controller: 'FeedbackCtrl'
          })
          .when('/profile', {
              templateUrl: '/user/profile',
              controller: 'FeedbackCtrl'
          })
          .when('/editprof', {
              templateUrl: '/user/editprof',
              controller: 'EditProfCtrl'
          })
          .when('/createprof', {
              templateUrl: '/user/send_profile',
              controller: 'CreateProfCtrl'
          })
          .when('/meters', {
              templateUrl: '/user/ViewMeters',
              controller: 'MetersCtrl'
          })
          .when('/addmeter',{
              templateUrl: '/user/addmeter',
             controller: 'MetersCtrl'
          })
          .when('/datameters', {
              templateUrl: '/user/ViewDataMeters',
              controller: 'ViewDataMetersCtrl'
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

/* Filter */
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


/* Main page */
HomeApp.controller('HomeCtrl', [
  '$scope', '$http', '$location', '$routeParams', 'Article',
  function ($scope, $http, $location, $routeParams, Article) {
      $scope.artId = $routeParams.artId;

      Article.get({ artId: $routeParams.artId }, function (data) {
          $scope.article = data;
      });
  }
]);







