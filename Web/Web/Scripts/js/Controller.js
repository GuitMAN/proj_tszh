;
'use strict';

/* Controllers */



var HomeApp = angular.module('HomeApp', ['ui.bootstrap', 'ngAnimate', 'ngRoute', 'ngResource', 'ngCookies', 'ngSanitize']);

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
  '$routeProvider', '$locationProvider', '$sceDelegateProvider',
function ($routeProvide, $locationProvider, $sceDelegateProvider, $httpProvider) {

    $sceDelegateProvider.resourceUrlWhitelist([
    // Allow same origin resource loads.
    'self',
    // Allow loading from our assets domain.  Notice the difference between * and **.
    'http://moe-tszh.ru**', 'http://localhost**'
    ]);

    //$httpProvider.defaults.headers.post = {
    //    'AccessControlAllowHeaders':
    //        'Content-Type, application/json, X-Requested-With, XMLHttpRequest'
    //};
    //$httpProvider.defaults.headers.post = {
    //    'withCredentials': true
    //};
      $routeProvide
          .when('/', {
              templateUrl: 'http://moe-tszh.ru/home/article',
              controller: 'HomeCtrl'
          })
          .when('/article/:artId', {
              templateUrl: 'http://moe-tszh.ru/home/article',
              controller: 'HomeCtrl'
          })
          .when('/login', {
              templateUrl: 'http://moe-tszh.ru/login/index',
              controller: 'LoginCtrl'
          })
          .when('/logoout', {
              templateUrl: 'http://moe-tszh.ru/login/logoout',
              controller: 'LogoutCtrl'
          })
          .when('/register', {
              templateUrl: 'http://moe-tszh.ru/login/register',
              controller: 'RegisterCtrl'
          })
          .when('/manage', {
              templateUrl: 'http://moe-tszh.ru/login/manage',
              controller: 'ManageCtrl'
          })
          .when('/feedback', {
              templateUrl: 'http://moe-tszh.ru/user/feedback',
              controller: 'FeedbackCtrl'
          })
          .when('/profile', {
              templateUrl: 'http://moe-tszh.ru/user/profile',
              controller: 'ProfileCtrl'
          })
          .when('/editprof', {
              templateUrl: 'http://moe-tszh.ru/user/editprof',
              controller: 'EditProfCtrl'
          })
          .when('/createprof', {
              templateUrl: 'http://moe-tszh.ru/user/send_profile',
              controller: 'CreateProfCtrl'
          })
          .when('/meters', {
              templateUrl: 'http://localhost:53574/user/ViewMeters',
              controller: 'MetersCtrl'
          })
          .when('/addmeter',{
              templateUrl: 'http://moe-tszh.ru/user/addmeter',
             controller: 'MetersCtrl'
          })
          .when('/datameters', {
              templateUrl: 'http://localhost:53574/user/ViewMeters',
              controller: 'ViewDataMetersCtrl'
          })
          .otherwise({
              redirectTo: 'http://moe-tszh.ru/home/'
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







