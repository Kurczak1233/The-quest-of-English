﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using The_quest_of_English.Models;
using The_quest_of_English.Models.ViewModels;
using The_quest_of_English.ViewModelMapper;
using TheEnglishQuest;
using TheEnglishQuestCore;
using TheEnglishQuestCore.Managers;

namespace The_quest_of_English
{
    [Area("Admin")]
    public class PlatformController : Controller
    {
        private readonly ApplicationUserViewModelMapper _applicationUserViewModelMapper;
        private readonly ApplicationUserManager _applicationUserManager;
        private readonly PlacementTestTaskViewModelMapper _placementTestTaskViewModelMapper;
        private readonly PlacementTestTaskManager _placementTestTaskManager;

        public int MyProperty { get; set; }
        public PlatformController(ApplicationUserViewModelMapper applicationUserViewModelMapper, ApplicationUserManager applicationUserManager, PlacementTestTaskViewModelMapper placementTestTaskViewModelMapper, PlacementTestTaskManager placementTestTaskManager)
        {
            _applicationUserViewModelMapper = applicationUserViewModelMapper;
            _applicationUserManager = applicationUserManager;
            _placementTestTaskViewModelMapper = placementTestTaskViewModelMapper;
            _placementTestTaskManager = placementTestTaskManager;
        }
        public async Task<IActionResult> MainView()
        {
            var userId = User.Identity.GetUserId();
            var user = await _applicationUserManager.GetLoggedUser(userId);
            var userViewModel = _applicationUserViewModelMapper.Map(user);
            return View(userViewModel);
        }
        public async Task<IActionResult> PlacementTest()
        {
            PlacementTestPointsAndPlacementTestTasksViewModel model = new PlacementTestPointsAndPlacementTestTasksViewModel();
            var TasksListDto = await _placementTestTaskManager.GetAllPositions();
            var TaskList = _placementTestTaskViewModelMapper.Map(TasksListDto);
            model.Tasks = TaskList;
            return View(model);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ActionName("PlacementTest")]
        public async Task<IActionResult> SubmitPoints(int points)
        {
            var userId = User.Identity.GetUserId();
            var TasksCount = _placementTestTaskManager.GetCount();
            double PointsPercentage = points / TasksCount;
            await _applicationUserManager.AssignLevel(PointsPercentage, userId);
            return RedirectToAction("MainView");
        }

        //Create
        public IActionResult CreateQuestion()
        {
            QuestionModelInput question = new QuestionModelInput();
            return View(question);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        [ActionName("CreateQuestion")]
        public IActionResult CreateQuestions(QuestionModelInput questions)
        {
            if (ModelState.IsValid)
            {
                AnswearAndQuestionsViewModel viewModel = new AnswearAndQuestionsViewModel();
                viewModel.Question = questions;
                return View("CreateAnswears", viewModel);
            }
            else
            {
                ModelState.AddModelError("Model", "Model is not valid!");
                return View();
            }
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ActionName("CreateAnswearsFunction")]
        public async Task<IActionResult> CreateAnswearsFunction(AnswearAndQuestionsViewModel questionsAndAnswears)
        {
            if (ModelState.IsValid)
            {
                PlacementTestBuilder builder = new PlacementTestBuilder();
                PlacementTestBuilderDirector director = new PlacementTestBuilderDirector(builder, questionsAndAnswears);
                PlacementTestTaskViewModel model = new PlacementTestTaskViewModel();
                model = director.BuildTask();
                var modelDto = _placementTestTaskViewModelMapper.Map(model);
                await _placementTestTaskManager.AddNewPosition(modelDto);
                return RedirectToAction("PlacementTest");
            }
            else
            {
                return RedirectToAction("MainView");
            }
        }
        // Modify
        public async Task<IActionResult> ModifyQuestion()
        {
            var TasksDto = await _placementTestTaskManager.GetAllPositions();
            var TaskViewModel = _placementTestTaskViewModelMapper.Map(TasksDto);
            return View(TaskViewModel);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ActionName("ModifyQuestion")]
        public IActionResult ModifyQuestionWithId(int id)
        {
            return RedirectToAction("ModifySpecifiedTask", new { id = id });
        }

        public async Task<IActionResult> ModifySpecifiedTask(int id)
        {
            var TaskDto = await _placementTestTaskManager.GetEntityById(id);
            var TaskViewModel = _placementTestTaskViewModelMapper.Map(TaskDto);
            return View(TaskViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [ActionName("ModifySpecifiedTask")]
        public async Task<IActionResult> ModifySpecifiedTaskPost(PlacementTestTaskViewModel model)
        {
            if (ModelState.IsValid)
            {
                PlacementTestTaskViewModel newModel = new PlacementTestTaskViewModel();
                newModel.QuestionFirstPart = model.QuestionFirstPart;
                newModel.QuestionSecondPart = model.QuestionSecondPart;
                newModel.QuestionDecoratedPart = model.QuestionDecoratedPart;
                newModel.FirstAnswear = model.FirstAnswear;
                newModel.SecondAnswear = model.SecondAnswear;
                newModel.ThirdAnswear = model.ThirdAnswear;
                newModel.FourthAnswear = model.FourthAnswear;
                newModel.CorrectAnswear = model.CorrectAnswear;
                var oldModelDto = await _placementTestTaskManager.GetEntityById(model.Id);
                await _placementTestTaskManager.DeletePosition(oldModelDto);
                var newModelDto = _placementTestTaskViewModelMapper.Map(newModel);
                await _placementTestTaskManager.AddNewPosition(newModelDto);


                return RedirectToAction("PlacementTest");
            }
            else
            {
                return RedirectToAction("PlacementTest");
            }
        }
        // Delete
        public async Task<IActionResult> DeleteQuestion()
        {
            var ListDto = await _placementTestTaskManager.GetAllPositions(); //Get List
            var List = _placementTestTaskViewModelMapper.Map(ListDto);
            return View(List);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteQuestion")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var Task = await _placementTestTaskManager.GetEntityById(id);
            await _placementTestTaskManager.DeletePosition(Task); //Deleting obj
            var ListDto = await _placementTestTaskManager.GetAllPositions(); //Get new list
            var List = _placementTestTaskViewModelMapper.Map(ListDto);
            return View(List);
        }
    }
}
