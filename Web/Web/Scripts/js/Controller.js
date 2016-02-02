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
              controller: 'LoginCtrl'
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

HomeApp.controller('ApplicationCtrl', function ($scope,
                                               USER_ROLES,
                                               AuthService) {
    $scope.currentUser = null;
    $scope.userRoles = USER_ROLES;
    $scope.isAuthorized = AuthService.isAuthorized;

  //  $scope.data = "data";
})




HomeApp.factory('AuthService', function ($http, Session) {
    return {
        login: function (credentials)
        {
            return $http.post('/Login?AspxAutoDetectCookieSupport=1', credentials)
              .then(function (res)
              {
                  Session.create(res.id, res.userid, res.role, "Факи");
                  return res;
              }),
                {
                }
              
        }//,
        //isAuthenticated: function () {
        //    return !!Session.userId;
        //},
        //isAuthorized: function (authorizedRoles) {
        //    if (!angular.isArray(authorizedRoles)) {
        //        authorizedRoles = [authorizedRoles];
        //    }
        //    return (this.isAuthenticated() &&
        //      authorizedRoles.indexOf(Session.userRole) !== -1);
        //}
    }
});

HomeApp.service('Session', function () {
    this.create = function (sessionId, userId, userRole, status) {
        this.id = sessionId;
        this.userId = userId;
        this.userRole = userRole;
        this.status = status;
    };
    this.destroy = function () {
        this.id = null;
        this.userId = null;
        this.userRole = null;
        this.status = null;
    };
    return this;
})

HomeApp.constant('AUTH_EVENTS', {
    loginSuccess: 'auth-login-success',
    loginFailed: 'auth-login-failed',
    logoutSuccess: 'auth-logout-success',
    sessionTimeout: 'auth-session-timeout',
    notAuthenticated: 'auth-not-authenticated',
    notAuthorized: 'auth-not-authorized'
})

.constant('USER_ROLES', {
    all: '*',
    admin: 'Admin',
    editor: 'Moder',
    guest: 'User'
})




HomeApp.controller('LoginCtrl', function ($scope, $rootScope, AUTH_EVENTS, AuthService) {
    $scope.credentials = {
        username: '',
        password: ''
    };
    
    $scope.login = function (credentials) {
        AuthService.login(credentials).then(function (data) {
            $rootScope.$broadcast(AUTH_EVENTS.loginSuccess);
            if (data.status==203)
            {
                $scope.data = "Ok";
                $scope.status = data.status;
            }
        }, function (data) {
            $rootScope.$broadcast(AUTH_EVENTS.loginFailed);
            $scope.status = Session.status;
            $scope.data = "no Ok";
        });
    };
})
          //function (login, answerForm)
      //{
      //    if (answerForm.$valid)
      //    {
      //        $http.post("/Login?AspxAutoDetectCookieSupport=1", $scope.login).then(function (data)
      //        {
      //            $scope.status = data.status;
      //            $scope.data = data.statusText;
      //            if (data.status==200)
      //                  $scope.autoApplications = "N";

      //        },function (data) {
      //            $scope.status = data.status;
      //            $scope.data = data.statusText;
      //        }) ;
      //    }
      //}
//  }
//]);