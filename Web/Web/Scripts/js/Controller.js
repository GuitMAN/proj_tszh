;
'use strict';

/* Controllers */
var HomeApp = angular.module('HomeApp', ['ngRoute', 'ngResource', 'ngCookies']);

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


HomeApp.controller('LoginInfoCtrl', function ($scope, $cookies, USER_ROLES, AuthService, $rootScope, Session)
{
    $scope.Session = Session;
    var curname = $cookies.get("username");
    var role = $cookies.get("Role");
    var id = $cookies.get("userId");
    if (curname) {
        Session.create(1, id, role, curname);
        $scope.userRoles = role;
        $scope.isAuthorized = AuthService.isAuthorized;
    }
    else {
        Session.destroy();
    }
});






function getHttpConfig() {
    var token = angular.element("input[name='__RequestVerificationToken']").val();
    var config = {
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            '__RequestVerificationToken': token
        }
    };
    return config;
}



HomeApp.factory('AuthService', function ($http, $cookies, Session) {
    return {
        login: function (credentials)
        {

            var config = getHttpConfig();
            return $http.post('/Login', credentials, config)
              .then(function (res)
              {             
                  Session.create(1, res.data.id, res.data.Role, res.data.Login);
                  return res;              
               })
              
        },
        register: function (reguser)
        {
            $http.post('/Login/Register', reguser)
                .then(function (data){
                if (data.status == 200) {
                    Session.create(1, data.data.id, data.data.Role, data.data.Login);
                }
                if (data.status == 203) {
                    Session.destroy();
                }
                return data;
            })
        },

        isAuthenticated: function () {
            return Session.userId;
        },
        isAuthorized: function (authorizedRoles) {
            if (!angular.isArray(authorizedRoles)) {
                authorizedRoles = [authorizedRoles];
            }
            return (Session.userId &&
              authorizedRoles.indexOf(Session.userRole) !== -1);
        }
    }
});


HomeApp.service('Session', function ($cookies) {
    this.create = function (sessionId, userId, userRole, currentUser) {
        this.id = sessionId;
        this.userId = userId;
        this.currentUser = currentUser;
        this.userRole = userRole;
        this.status = status;
        $cookies.put('userId', userId);
        $cookies.put('username', currentUser);
        $cookies.put('userRole', userRole)

    };
    this.destroy = function () {
        $cookies.remove('userId');
        $cookies.remove('username');
        $cookies.remove('userRole');
        $cookies.remove('__RequestVerificationToken')
        this.id = null;
        this.userId = null;
        this.userRole = null;
        this.status = null;
        this.currentUser = null;
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

HomeApp.constant('USER_ROLES', {
    all: '*',
    admin: 'Admin',
    editor: 'Moder',
    guest: 'User'
})


HomeApp.controller('LoginCtrl', function ($scope, $rootScope, AUTH_EVENTS, AuthService, Session, $location) {
    $scope.credentials = {
        username: '',
        password: '',
        RememberMe: false
    };
    $scope.login = function (credentials) {
        AuthService.login(credentials)
            .then(function (data)
            {             
                if (data.status == 200) {
                    $rootScope.$broadcast(AUTH_EVENTS.loginSuccess);

                } else
                {
                //    $rootScope.$broadcast(AUTH_EVENTS.auth-login-failed)
                    Session.destroy()
                }
            
        });
    };
   
})

HomeApp.controller('LogoutCtrl', function ($http, Session, $location) {
    $http.post('/Login/Logoout').then(function ()
    {
      //  $rootScope.$broadcast(AUTH_EVENTS.auth-logout-success);
        Session.destroy(); 

    });
    return  $location.path('#/');
})

HomeApp.controller('RegisterCtrl', function ($scope, $rootScope, AUTH_EVENTS, AuthService, Session) {
    $scope.reguser = {
        UserName: '',
        Password: '',
        ConfirmPassword: ''
    };
    $scope.register = function (reguser) {
        AuthService.register(reguser)
            .then(function (data) {
                //if (data.status == 200) {
                    
                ////    $rootScope.$broadcast(AUTH_EVENTS.loginSuccess);
                //} else {
                //    //    $rootScope.$broadcast(AUTH_EVENTS.auth-login-failed)
                //    Session.destroy()
                //}
            });
    };

})
