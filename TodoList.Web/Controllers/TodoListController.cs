﻿using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TodoList.Web.Filters;
using TodoList.Web.Helpers;
using TodoList.Web.Models;

namespace TodoList.Web.Controllers
{
    [BasicAndFormsAuthentication]
    public class TodoListController : ApiController
    {
        private readonly TodoListContext _db = new TodoListContext();

        // GET api/TodoList
        public IEnumerable<TodoItem> GetTodoItems()
        {
            return _db.TodoItems.Where(x => x.User.Username == HttpContext.Current.User.Identity.Name).AsEnumerable();
        }

        // GET api/TodoList/5
        public TodoItem GetTodoItem(int id)
        {
            var todoitem = _db.TodoItems.SingleOrDefault(x => x.Id == id &&
                                                             x.User.Username == HttpContext.Current.User.Identity.Name);
            if (todoitem == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return todoitem;
        }

        // PUT api/TodoList/
        public HttpResponseMessage PutTodoItem(TodoItem todoitem)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, 
                                              new { errors = ModelStateHelpers.GetErrorsFromModelState(ModelState) });
            }

            _db.Entry(todoitem).State = EntityState.Modified;

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/TodoList
        public HttpResponseMessage PostTodoItem(TodoItem todoitem)
        {
            if (ModelState.IsValid)
            {
                var user = _db.Users.Single(x => x.Username == HttpContext.Current.User.Identity.Name);

                todoitem.UserId = user.Id;

                _db.TodoItems.Add(todoitem);
                _db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.Created, todoitem);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest,
                                          new { errors = ModelStateHelpers.GetErrorsFromModelState(ModelState) });
        }

        // DELETE api/TodoList/5
        public HttpResponseMessage DeleteTodoItem(int id)
        {
            var todoitem = _db.TodoItems.SingleOrDefault(x => x.Id == id &&
                                                             x.User.Username == HttpContext.Current.User.Identity.Name);
            if (todoitem == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            _db.TodoItems.Remove(todoitem);

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, todoitem);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}