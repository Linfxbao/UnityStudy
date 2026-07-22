#import "alta-typst.typ": alta, term, skill, styled-link, name, target

#alta(
  name: "王博",
  photo: "./Photo.jpg",
  links: (
    (name: "birth", link:"2005.04.16", display: "2005.04.16"),
    (name: "email", link: "mailto:2847006075@qq.com"),
    (name: "home", link: "https://www.bjtu.edu.cn/", display: "北京市海淀区"),
    (name: "github", link: "https://github.com/vaboe", display: "@vaboe"),
    (name: "phonenumber", link: "tel:19351461312", display: "19351461312"),
  ),
  // tagline: [Software Engineer. #lorem(10)],
  context [
    == 教育背景

  
    #name[BJTU-北京交通大学]
    #term[2023.09 --- 至今][北京.海淀]

    计算机科学与技术(本科)#h(6.5em)专业成绩: 3.2/4.0
    Java语言程序设计(92)、 C语言程序设计(90)\ 汇编与接口技术(86)...

      // B.Sc. Computer Science with Year in Industry. #lorem(10)

    // === A-Levels \
    // #name[Place Grammar School]
    // #term[Sep 2017 --- Aug 2019][Place, UK]

    // #lorem(10)

    == 获奖情况

    // === Junior Software Engineer \
    // #name[_Company Two_]
    // #term[Jul 2021 --- Jul 2022][London, UK]
    - 2023 --- 2024学年优秀团员
    - 2024 --- 2025学年优秀团干部
    - 2024年北京大学生艺术节 #h(9.5em)金奖
    - 2025全球数字经济创新大赛·AIGC创作大赛·CSDN分赛道 #h(8em)优秀奖 #h(4em)2025年10月
    

    // // - #lorem(15)
    // // - #lorem(10)
    // // - #lorem(24)

    // === Junior Systems Administrator \
    // #name[Company 1]
    // #term[Oct 2019 --- Jul 2021][Bournemouth, UK]

    // - #lorem(20)
    // - #lorem(10)

    // References available on request

    == 校园经历
    - 2023.09 --- 2024.06 北交民族管弦乐团 团员\
    - 2024.09 --- 2025.06 北交民族管弦乐团 财务部长\
    - 在校期间多次参加各种校园活动和演出，在乐团内演奏低音笙；同时积极参与运动会等体育活动并获得奖项。\

    // - Walks on the beach
    // - #lorem(8)
    // - #lorem(6)
    // - #lorem(4)
    // - #lorem(12)
    
    == 自我评价
    - 具有较强的学习能力和适应能力，能够快速掌握新知识和技能；\
    - 具备良好的沟通能力和团队合作精神，能够积极与人合作完成任务；\
    - 具有较强的责任心和工作热情，能够认真负责地完成工作任务；\
    - 具有强烈的学习兴趣和求知欲，希望能够不断提升自己的能力和水平；\
    - 具有较强的抗压能力和解决问题的能力。

    == 技能

    #skill("C#", 4)
    #skill("Unity 6", 4)
    #skill("Cinemachine", 3)
    #skill("NavMesh AI", 3)
    #skill("CharacterController", 3)
    #skill("UGUI", 3)
    #skill("Git", 2)

    #if target() == "paged" {
      colbreak()
    }

    == 项目经历

    ==== ZombieLand（3D 生存射击游戏）


    核心技术: Unity 6、C\#、CharacterController、Cinemachine、NavMesh、UGUI

    项目描述: \
    基于 Unity 6 开发 3D 第三人称生存射击游戏，完成 MainMenu、角色选择与 ZombieLand 主场景流程搭建，实现角色移动、冲刺、跳跃、近战与步枪切换、
    第三人称/瞄准双视角切换、生命值与受伤反馈、弹药与装弹 UI、僵尸战斗及任务推进等核心玩法，形成较完整的单人闯关生存体验。
    
    系统实现: 基于 CharacterController 与 Animator 搭建玩家移动状态机，通过 PlayerScript、SwitchCamera 处理行走、奔跑、跳跃、受伤、死亡以及第三人称视角与 AimCam 的切换逻辑；\
    使用 Rifle、Punch、Weapon、RifleUI 实现近战开局、武器拾取、步枪射击、命中判定、换弹、弹药显示及枪口特效/命中特效/音效反馈；\
    结合 Health、HealthBar、RotateHealthBarUI、FootStepSound 等脚本完善玩家与敌人生命条、受击表现及基础交互反馈。
    ==== 任务推进、僵尸 AI 与载具逃生系统开发

    核心技术: Unity 6、C\#、NavMeshAgent、WheelCollider、SceneManagement、UGUI

    项目描述: \
    围绕 ZombieLand 主场景扩展任务目标、区域刷怪、补给拾取、载具驾驶与多角色入口，构建从搜集武器、对抗僵尸到驾驶车辆撤离的完整关卡推进流程，提升关卡叙事与玩法层次。
    
    系统实现: 基于 MainMenuController、SelectCharacterController、ObjectiveController、MenuController 等脚本实现主菜单、角色选择、多场景进入与任务文本变色反馈；\
    使用 Zombie1、Zombie2、ZombieSpawn 结合 NavMeshAgent、巡逻点、视野/攻击半径与区域触发器，实现僵尸巡逻、追击、攻击、死亡及持续刷怪逻辑；\
    通过 VehicleController 配置 WheelCollider 车辆移动、转向、制动和上下车切换，并在驾驶状态下关闭玩家模型与视角 UI，实现载具碾压僵尸与任务完成联动。

    // ==== Employee Appraisal System

    // - #lorem(15)
    // - #lorem(10)
    // - #lorem(20)

    // ==== Hackathons

    // / Hack1: #lorem(20)
    // / TwoHackTwo: #lorem(15)

    // ==== Project 4

    // - #lorem(30)
    // - #lorem(10)

    // ==== Project Five

    // - #lorem(23)

    
  ],
)





