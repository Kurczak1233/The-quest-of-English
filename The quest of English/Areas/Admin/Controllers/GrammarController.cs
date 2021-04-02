﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using The_quest_of_English.Models;
using The_quest_of_English.ViewModelMapper;
using TheEnglishQuestCore;
using TheEnglishQuestCore.Managers;

namespace The_quest_of_English.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class GrammarController : Controller
    {
        private readonly GrammarQuizViewModelMapper _grammarQuizViewModelMapper;
        private readonly GrammarQuizManager _grammarQuizManager;
        private readonly ApplicationUserViewModelMapper _applicationUserViewModelMapper;
        private readonly ApplicationUserManager _applicationUserManager;
        private readonly GrammarTaskManager _grammarTaskManager;
        private readonly GrammarTaskViewModelMapper _grammarTaskViewModelMapper;


        public GrammarController(GrammarQuizViewModelMapper gvmm, GrammarQuizManager gq,
            ApplicationUserViewModelMapper uservmm, ApplicationUserManager usermanager,
            GrammarTaskManager grammarTaskManager, GrammarTaskViewModelMapper grammarTaskViewModelMapper)
        {
            _applicationUserViewModelMapper = uservmm;
            _applicationUserManager = usermanager;
            _grammarQuizManager = gq;
            _grammarQuizViewModelMapper = gvmm;
            _grammarTaskManager = grammarTaskManager;
            _grammarTaskViewModelMapper = grammarTaskViewModelMapper;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> FCE()
        {
            var QuizesList = await _grammarQuizManager.GetAllQuizzesFiltered(SD.FCE);
            var QuizzesViewModel = _grammarQuizViewModelMapper.Map(QuizesList);
            return View(QuizzesViewModel);
        }
        public async Task<IActionResult> CAE()
        {

            var QuizesList = await _grammarQuizManager.GetAllQuizzesFiltered(SD.CAE);
            var QuizzesViewModel = _grammarQuizViewModelMapper.Map(QuizesList);
            return View(QuizzesViewModel);
        }
        public async Task<IActionResult> CPE()
        {
            var QuizesList = await _grammarQuizManager.GetAllQuizzesFiltered(SD.CPE);
            var QuizzesViewModel = _grammarQuizViewModelMapper.Map(QuizesList);
            return View(QuizzesViewModel);
        }

        public async Task<IActionResult> GrammarCreateQuiz()
        {
            GrammarQuizViewModel quiz = new GrammarQuizViewModel();
            var userId = User.Identity.GetUserId();
            var user = await _applicationUserManager.GetLoggedUser(userId);
            var userViewModel = _applicationUserViewModelMapper.Map(user);
            quiz.User = userViewModel;
            return View(quiz);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("GrammarCreateQuiz")]
        public async Task<IActionResult> GrammarCreateQuizPost(GrammarQuizViewModel quiz)
        {
            if (ModelState.IsValid)
            {
                var quizes = await _grammarQuizManager.GetAllQuizzes();
                var quizesVM = _grammarQuizViewModelMapper.Map(quizes);
                foreach(var item in quizesVM)
                {
                    if(item.Name == quiz.Name)
                    {
                        ModelState.AddModelError("Name", "There already is a quiz named like that!");
                        return View("GrammarCreateQuiz");
                    }
                    //Else there is no name like that -- continue.
                }
                //
                var userId = User.Identity.GetUserId();
                var quizDto =  _grammarQuizViewModelMapper.Map(quiz);
                await _grammarQuizManager.AddNewQuiz(quizDto, userId);
                await _grammarQuizManager.RemoveQuiz(quizDto);
                //Getting Quiz from DB with assigned Id
                var quizVM = await _grammarQuizManager.FindQuizByName(quiz.Name);
                return RedirectToAction("ShowQuiz", new { quizId = quizVM.Id }); //Name of variable must be inside annonymus obj!
            }
            else
            {
                return RedirectToAction("CAE");
            }
        }
        public async Task<IActionResult> ShowQuiz(int quizId)
        {
            //Get User
            var quiz = await _grammarQuizManager.FindQuiz(quizId);
            var quizVM = _grammarQuizViewModelMapper.Map(quiz);
            var userId = User.Identity.GetUserId();
            var user = await _applicationUserManager.GetLoggedUser(userId);
            var userVm = _applicationUserViewModelMapper.Map(user);
            QuizModelAndUserViewModel model = new QuizModelAndUserViewModel()
            {
                Quiz = quizVM,
                ApplicationUser = userVm,
            };
            return View(model); 
        }

        public async Task<IActionResult> GrammarModifyQuiz(string level)
        {
            var QuizesList = await _grammarQuizManager.GetAllQuizzesFiltered(level);
            var QuizzesViewModel = _grammarQuizViewModelMapper.Map(QuizesList);
            return View(QuizzesViewModel);
        }

        public async Task<IActionResult> ModifySpecifiedQuiz(int id)
        {
            var Quizes = await _grammarQuizManager.GetAllQuizzes();
            var Quiz = Quizes.Where(x => x.Id == id).SingleOrDefault();
            var QuizVm = _grammarQuizViewModelMapper.Map(Quiz);
            return View(QuizVm);
        }

        public async Task<IActionResult> ModifySpecifiedQuiz()
        {
            return RedirectToAction();

        }

        public async Task<IActionResult> GrammarDeleteQuiz(string level)
        {
            var QuizesList = await _grammarQuizManager.GetAllQuizzesFiltered(level);
            var QuizzesViewModel = _grammarQuizViewModelMapper.Map(QuizesList);
            return View(QuizzesViewModel);
        }

        //TASKS SECTION
        public IActionResult CreateTask(int quizId)
        {            
            GrammarTasksViewModel task = new GrammarTasksViewModel();
            task.GrammarQuizId = quizId;
            return View(task);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("CreateTask")]
        public  IActionResult CreateTaskPost(GrammarTasksViewModel task)
        {
            return RedirectToAction("BuildTaskQuestionAndAnswear" , new { ChosenType = task.TaskType, QuizId = task.GrammarQuizId });
        }


        public IActionResult BuildTaskQuestionAndAnswear(string ChosenType, int quizId)
        {
            GrammarTasksViewModel task = new GrammarTasksViewModel
            {
                GrammarQuizId = quizId,
                TaskType = ChosenType
            };
            return View(task);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("BuildTaskQuestionAndAnswear")]
        public async Task<IActionResult> BuildTaskQuestionAndAnswearPost(GrammarTasksViewModel task)
        {
            var taskDto = _grammarTaskViewModelMapper.Map(task);
            await _grammarTaskManager.AddNew(taskDto);
            return RedirectToAction("ShowQuiz", new { quizId = task.GrammarQuizId });
        }


    }
}
